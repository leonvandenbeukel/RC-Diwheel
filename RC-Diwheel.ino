/*
 * MIT License
 * 
 * Copyright (c) 2019 Leon van den Beukel
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * Source: 
 * https://github.com/leonvandenbeukel/RC-Diwheel 
 * 
 */

#include <SoftwareSerial.h>

SoftwareSerial BTSerial(11, 12); // RX, TX
String btBuffer;
 
#define Ain1 9
#define Ain2 10
#define Bin1 6
#define Bin2 5

void setup() {

  Serial.begin(115200);         // Debug
  BTSerial.begin(9600);         // Bluetooth
  
  pinMode(Ain1, OUTPUT);        // A in1
  pinMode(Ain2, OUTPUT);        // A in2
  pinMode(Bin1, OUTPUT);        // B in1
  pinMode(Bin2, OUTPUT);        // B in2

  analogWrite(Ain1,0);
  analogWrite(Ain2,0);
  analogWrite(Bin1,0);
  analogWrite(Bin2,0);

}

void loop() {

  if (BTSerial.available())
  {
    char received = BTSerial.read();
    btBuffer += received; 
    if (received == '|')
    {
        processCommand();
        btBuffer = "";
    }
  }    
}

void processCommand() {
  char separator = ',';
  int percL = getValue(btBuffer, separator, 0).toInt();
  int percR = getValue(btBuffer, separator, 1).toInt();

  if (percL > 0) {
    int spd = map(percL, 0, 100, 100, 255);
    analogWrite(Ain1, spd);  
    analogWrite(Ain2, 0);
    // Serial.print("Left > 0: ");
    // Serial.println(spd);
  } else if (percL < 0) {
    int spd = map(percL, -100, 0, 255, 100);
    analogWrite(Ain1, 0);  
    analogWrite(Ain2, spd);
    // Serial.print("Left < 0: ");
    // Serial.println(spd);
  } else if (percL == 0) {
    analogWrite(Ain1, 0);
    analogWrite(Ain2, 0);
  } 

  if (percR > 0) {
    int spd = map(percR, 0, 100, 100, 255);
    analogWrite(Bin1, spd);  
    analogWrite(Bin2, 0);
    // Serial.print("Right > 0: ");
    // Serial.println(spd);
  } else if (percR < 0) {
    int spd = map(percR, -100, 0, 255, 100);
    analogWrite(Bin1, 0);  
    analogWrite(Bin2, spd);
    // Serial.print("Right < 0: ");
    // Serial.println(spd);
  } else if (percR == 0) {
    analogWrite(Bin1, 0);
    analogWrite(Bin2, 0);
  } 

  delay(10);  
}


String getValue(String data, char separator, int index) {
  int found = 0;
  int strIndex[] = {0, -1};
  int maxIndex = data.length()-1;

  for(int i=0; i<=maxIndex && found<=index; i++){
    if(data.charAt(i)==separator || i==maxIndex){
        found++;
        strIndex[0] = strIndex[1]+1;
        strIndex[1] = (i == maxIndex) ? i+1 : i;
    }
  }
  return found>index ? data.substring(strIndex[0], strIndex[1]) : "";
}
