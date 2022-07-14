#include <Arduino.h>
#include <util/atomic.h>

#define PPR 500 // DC Motor Encoder Accuracy
#define ENCA 2  // DC Motor Encoder Out1
#define ENCB 3  // DC Motor Encoder Out2

#define SAMPLINGPRIODE 1  // Sampling Rate
#define ENA 13   // Motor Driver PWM Input
#define IN1 11   // Motor Driver In1 Input
#define IN2 12   // Motor Driver In2 Input

int direction = 1;
double Kp = 0.0406812507453938;
double Ki = 0.339782106923304;
double Kd = 3.14697363162208e-5;
long int targetSpeed = 10*PPR;
double integrral = 0;

unsigned long int currentTime = 0;
unsigned long int previousTime = 0;

long int pos = 0;
long int previousError = 0;

unsigned long int startTime = 0;

void updateSetting(){
  String data = Serial.readString();
  int sep = 0;
  direction = data.substring(sep, data.indexOf('|')).toInt();
  sep = data.indexOf('|', sep+1);
  Kp = data.substring(sep+1, data.indexOf('|', sep+1)).toDouble();
  sep = data.indexOf('|', sep+1);
  Ki = data.substring(sep+1, data.indexOf('|', sep+1)).toDouble();
  sep = data.indexOf('|', sep+1);
  Kd = data.substring(sep+1, data.indexOf('|', sep+1)).toDouble();
  sep = data.indexOf('|', sep+1);
  targetSpeed = data.substring(sep+1, data.indexOf('|', sep+1)).toDouble();
}

// checks if chanel a raised earlier then count up position
// because it is cw rotation else its ccw rotation and counts down
void encoderInterrupt(){
  int b = digitalRead(ENCB);
  if(b==0){
    pos++;
  }
  else{
    pos--;
  }
}

// 
void setDirection(){
  if(direction != 0){
    digitalWrite(IN1, HIGH);
    digitalWrite(IN2, LOW);
  }
  else{
    digitalWrite(IN1, LOW);
    digitalWrite(IN2, HIGH);
  }
}

void setup() {
  Serial.begin(9600);
  pinMode(ENCA, INPUT_PULLUP);
  pinMode(ENCB, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(ENCA), encoderInterrupt, RISING);

  pinMode(ENA, OUTPUT);
  pinMode(IN1, OUTPUT);
  pinMode(IN2, OUTPUT);
  startTime = micros();
}

void loop() {
  if(Serial.available()) {
    updateSetting();
  }
  long int currentPos = pos;
  pos = 0;
  currentTime = micros() - startTime;
  // report current speed
  Serial.println(String(currentPos, DEC)+"|"+String(currentTime/1000, DEC));
  long int error_value = currentPos - targetSpeed;
  // product term
  double p = Kp*error_value;
  // passed time
  double dt = (currentTime-previousTime)/1e6;
  // integrate term    
  integrral = integrral + error_value*dt;
  double i = Ki*(integrral);
  // derivative term
  double d = Kd*(error_value-previousError)/dt;
  long int pwm = floor(p)+floor(i)+floor(d);
  if(pwm<0){
    direction = (direction == 0) ? 1: 0;
    pwm = abs(pwm);
  }
  int pw = map(pwm, 0, targetSpeed, 0, 255);
  previousTime = currentTime;
  previousError = error_value;
  analogWrite(ENA, pw);
  setDirection();
  delay(SAMPLINGPRIODE);
}