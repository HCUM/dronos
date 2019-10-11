// #include <Arduino.h>
#include <WiFi.h>
#include <WiFiAP.h>
#include <driver/rmt.h>
#include <iterator>
#include <vector>
#include "soc/rtc.h"
using namespace std;

TaskHandle_t Task0;
TaskHandle_t Task1;
hw_timer_t *timer = NULL;

volatile SemaphoreHandle_t timerSemaphore;

#define DIVIDER 8     
#define DURATION 12.5

#define GPIO_PIN1 16
#define GPIO_PIN2 17
#define GPIO_PIN3 18

#define PULSE_T0H ((625 * 4) / (DURATION * DIVIDER));
#define PULSE_T1H ((1250 * 4) / (DURATION * DIVIDER));
#define PULSE_T0L ((1045 * 4) / (DURATION * DIVIDER));
#define PULSE_T1L ((420 * 4) / (DURATION * DIVIDER));

#define PULSE_LOOOOOL ((1500 * 1000) / (DURATION * DIVIDER));

const int lowPulse = 3009;
#define SIZE 9

void recvWithStartEndMarkers();
void parseData();
void getSerialData();

portMUX_TYPE timerMux0 = portMUX_INITIALIZER_UNLOCKED;
// portMUX_TYPE timerMux1 = portMUX_INITIALIZER_UNLOCKED;

unsigned long previousMicros = 0;
const long interval = 22500;
const long intervalDelay = 18000;
unsigned long currentMicros = 0;

vector<int> pulselengthsPPM = {15000, 12000, 10000, 10000, 10000, 11000, 12000, 13000};
vector<int> pulselengthsPPM_copy = {10000, 15000, 15000, 15000, 10000, 11000, 12000, 13000};

vector<int> defaultPulselengths = {10000, 15000, 15000, 15000, 10000, 10000, 10000, 10000};
int values[] = {0, 0, 0, 0, 0, 0, 0, 0};

rmt_config_t config;
rmt_item32_t *Times_Array = (rmt_item32_t *)malloc(SIZE);

volatile bool rmtDone = false;
volatile bool rmtNOW = false;
unsigned long lastMillis = 0;
const int chCount = 8;

// ********** Serial Receive *******
static boolean recvInProgress = false;
static byte ndx = 0;
char startMarker = '<';
char endMarker = '>';
char rc;
const byte numChars = 48;
char receivedChars[numChars];
char tempChars[numChars]; // temporary array for use when parsing
size_t destination_size;
// ******************************

boolean newData = false;

int calcDuration(int microSeconds)
{
    return (microSeconds / (DURATION * DIVIDER)) * 100;
}

void computePPM_RMT()
{
    for (int i = 0; i < pulselengthsPPM_copy.size(); i++)
    {
        Times_Array[i].duration0 = calcDuration(lowPulse);
        Times_Array[i].level0 = 0;
        Times_Array[i].duration1 = calcDuration(pulselengthsPPM_copy.at(i) - lowPulse);
        Times_Array[i].level1 = 1;
    }

    Times_Array[pulselengthsPPM_copy.size()].duration0 = calcDuration(lowPulse);
    Times_Array[pulselengthsPPM_copy.size()].level0 = 0;
    Times_Array[pulselengthsPPM_copy.size()].duration1 = 0;
    Times_Array[pulselengthsPPM_copy.size()].level1 = 1;
}

void setupRMT()
{
    config.rmt_mode = RMT_MODE_TX;
    config.channel = RMT_CHANNEL_0;
    config.gpio_num = (gpio_num_t)GPIO_PIN1;
    config.mem_block_num = 1;
    config.tx_config.loop_en = 0;
    config.tx_config.carrier_en = 0;
    config.tx_config.idle_output_en = 1;
    config.tx_config.idle_level = RMT_IDLE_LEVEL_HIGH;
    config.clk_div = DIVIDER;

    ESP_ERROR_CHECK(rmt_config(&config));
    ESP_ERROR_CHECK(rmt_driver_install(config.channel, 0, 0));
}

void IRAM_ATTR onTimer()
{
    xSemaphoreGiveFromISR(timerSemaphore, NULL);
}

void createLoopOnCore0(void *parameter)
{
    for (;;)
    {
        if (xSemaphoreTake(timerSemaphore, 0) == pdTRUE)
        {
            computePPM_RMT();

            digitalWrite(GPIO_PIN2, 1);

            rmt_write_items(RMT_CHANNEL_0, Times_Array, SIZE, true);
            rmt_wait_tx_done(config.channel, portMAX_DELAY);

            digitalWrite(GPIO_PIN2, 0);
            rmtDone = true;
            // Serial1.println("lol test123");
        }
    }

    //     config.rmt_mode = RMT_MODE_TX;
    //     config.channel = RMT_CHANNEL_0;
    //     config.gpio_num = (gpio_num_t)GPIO_PIN1;
    //     config.mem_block_num = 1;
    //     config.tx_config.loop_en = 0;
    //     config.tx_config.carrier_en = 0;
    //     config.tx_config.idle_output_en = 1;
    //     config.tx_config.idle_level = RMT_IDLE_LEVEL_HIGH;
    //     config.clk_div = DIVIDER;

    //     ESP_ERROR_CHECK(rmt_config(&config));
    //     ESP_ERROR_CHECK(rmt_driver_install(config.channel, 0, 0));

    //     while (1)
    //     {

    //         currentMicros = micros();

    //         if (currentMicros - previousMicros >= interval)
    //         {

    //             // portENTER_CRITICAL_ISR(&timerMux0);
    //             // // std::copy(std::begin(pulselengthsPPM), std::end(pulselengthsPPM), std::begin(pulselengthsPPM_copy));
    //             // portEXIT_CRITICAL_ISR(&timerMux0);
    //             portENTER_CRITICAL_ISR(&timerMux0);

    //             computePPM_RMT();

    //             previousMicros = currentMicros;

    //             digitalWrite(GPIO_PIN2, 1);
    //             portEXIT_CRITICAL_ISR(&timerMux0);

    //             rmt_write_items(RMT_CHANNEL_0, Times_Array, SIZE, true);
    //             rmt_wait_tx_done(config.channel, portMAX_DELAY);

    //             digitalWrite(GPIO_PIN2, 0);

    //             portENTER_CRITICAL_ISR(&timerMux0);
    //             rmtDone = true;
    //             portEXIT_CRITICAL_ISR(&timerMux0);
    //         }
    //         // if (currentMicros - previousMicros >= intervalDelay)
    //         // {
    //         //     delay(2);
    //         // }
    //     }
}

void createLoopOnCore1(void *parameter)
{
    for (;;)
    {

        // if rmt has sent signal, look for new data on serial port
        // if longer than 250 ms no signal came, put output to default (all motors off)
        if (rmtDone)
        {
            portENTER_CRITICAL(&timerMux0);

            // portEXIT_CRITICAL_ISR(&timerMux0);

            getSerialData();

            // portENTER_CRITICAL_ISR(&timerMux0);
            rmtDone = false;
            portEXIT_CRITICAL(&timerMux0);

            if (millis() - lastMillis > 250)
            {
                for (int q = 0; q < chCount; q++)
                {
                    pulselengthsPPM.at(q) = defaultPulselengths.at(q);
                }
            }

            portENTER_CRITICAL(&timerMux0);
            pulselengthsPPM_copy = pulselengthsPPM;
            portEXIT_CRITICAL(&timerMux0);
        }

        // delayMicroseconds(800);
    }
}

void setup()
{
    Serial.begin(115200);
    Serial1.begin(115200, SERIAL_8N1, 4, 2);
    // Serial1.setDebugOutput(true);

    esp_log_level_set("*", ESP_LOG_DEBUG);

    pinMode(GPIO_PIN2, OUTPUT);
    pinMode(GPIO_PIN3, OUTPUT);
    digitalWrite(GPIO_PIN3, 1);
    digitalWrite(GPIO_PIN3, 0);

    WiFi.mode(WIFI_OFF);
    btStop();
    rtc_clk_cpu_freq_set(RTC_CPU_FREQ_240M);

    periph_module_disable(PERIPH_RMT_MODULE);
    periph_module_enable(PERIPH_RMT_MODULE);
    // computePPM_RMT();

    setupRMT();

    timerSemaphore = xSemaphoreCreateBinary();

    timer = timerBegin(3, 80, true);
    timerAttachInterrupt(timer, &onTimer, true);
    timerAlarmWrite(timer, 22500, true);
    timerAlarmEnable(timer);

    xTaskCreatePinnedToCore(
        createLoopOnCore0,
        "rmt_task",
        2048,
        NULL,
        2,
        &Task0,
        0);

    xTaskCreatePinnedToCore(
        createLoopOnCore1,
        "serial_task",
        2048,
        NULL,
        2,
        &Task1,
        1);
}

void getSerialData()
{

    recvWithStartEndMarkers();
    if (newData == true)
    {
        // strcpy(tempChars, receivedChars);
        destination_size = sizeof(receivedChars);

        strncpy(tempChars, receivedChars, destination_size);
        tempChars[destination_size - 1] = '\0';

        // this temporary copy is necessary to protect the original data
        //   because strtok() used in parseData() replaces the commas with \0
        // Serial.println(receivedChars);
        if (receivedChars[destination_size - 1] == '\0')
        {
            parseData();
        }else{
            Serial.print("Error whilst parsing:   ");
            Serial.print(receivedChars);
            Serial.print(".........");
            Serial.print(tempChars);
            Serial.println();
        }
        newData = false;

        Serial.println(values[0]);

        for (int q = 0; q < chCount; q++)
        {
            pulselengthsPPM[q] = values[q] * 10;
        }

        lastMillis = millis();

        //delay(5);
    }
}

void loop()
{
}

void recvWithStartEndMarkers()
{
    digitalWrite(GPIO_PIN2, 1);

    while (Serial1.available() > 0 && newData == false)
    {
        rc = Serial1.read();
        // Serial1.write(rc);

        if (recvInProgress == true)
        {
            if (rc != endMarker)
            {
                receivedChars[ndx] = rc;
                ndx++;
                if (ndx >= numChars)
                {
                    ndx = numChars - 1;
                }
            }
            else
            {
                receivedChars[ndx] = '\0'; // terminate the string
                recvInProgress = false;
                ndx = 0;
                newData = true;
                digitalWrite(GPIO_PIN2, 0);
                // Serial1.flush();
            }
        }

        else if (rc == startMarker)
        {
            recvInProgress = true;
        }
    }
    // just for debugging with logic analyser
    digitalWrite(GPIO_PIN2, 0);
}

//============

void parseData()
{ // split the data into its parts

    char *strtokIndx; // this is used by strtok() as an index

    // Serial.print("tempchar: ");
    // Serial.print(tempChars);
    // Serial.println();

    strtokIndx = strtok(tempChars, ","); // get the first part - the string
    values[0] = atoi(strtokIndx);

    strtokIndx = strtok(NULL, ","); // this continues where the previous call left off
    values[1] = atoi(strtokIndx);   // convert this part to an integer

    strtokIndx = strtok(NULL, ",");
    values[2] = atoi(strtokIndx); // convert this part to a float

    strtokIndx = strtok(NULL, ",");
    values[3] = atoi(strtokIndx); // convert this part to a float

    strtokIndx = strtok(NULL, ",");
    values[4] = atoi(strtokIndx); // convert this part to a float

    strtokIndx = strtok(NULL, ",");
    values[5] = atoi(strtokIndx); // convert this part to a float

    strtokIndx = strtok(NULL, ",");
    values[6] = atoi(strtokIndx); // convert this part to a float

    strtokIndx = strtok(NULL, ",");
    values[7] = atoi(strtokIndx); // convert this part to a float
}
