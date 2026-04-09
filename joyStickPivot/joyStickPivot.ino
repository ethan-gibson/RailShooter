#include "Mouse.h"
#include "Keyboard.h"

const int yAxis = A0;       // Joystick X-axis
const int xAxis = A1;       // Joystick Y-axis

int triggerPin = 10;
int reloadPin = 9;
int fireSafety = 4;
int fireSemi = 5;
int fireBurst = 6;
int fireAuto = 7;
int hapticPin = 13;

int range = 12;             // The max cursor movement per loop (4-12 is a good range)
int responseDelay = 5;      // Delay (in ms) between mouse movements
int threshold = range / 6;  // The joystick's "dead zone" to prevent drift
int center = range / 2;     // The joystick's center position

int ammoCount = 20;
const int maxAmmo = 20;

bool burstComplete = false;
bool magOut = false;

int lastTriggerState = HIGH;
unsigned long lastFireTime = 0;
const unsigned int burstDelay = 150;
const unsigned int autoFireRate = 200;

void fireOneShot();

void setup() {
  delay(5000);
  pinMode(triggerPin, INPUT_PULLUP);
  pinMode(reloadPin, INPUT_PULLUP);
  pinMode(fireSafety, INPUT_PULLUP);
  pinMode(fireSemi, INPUT_PULLUP);
  pinMode(fireAuto, INPUT_PULLUP);
   Mouse.begin();
  Keyboard.begin();
}

void loop() {
  int triggerState = digitalRead(triggerPin);
  int mode = 3; // 0=safety, 1=semi, 2=burst, 3=auto

  // Joystick Logic

  int xReading = analogRead(xAxis);
  int yReading = analogRead(yAxis);

  int xMapped = map(xReading, 0, 1023, -range, range);
  int yMapped = map(yReading, 0, 1023, -range, range);

  if(abs(xMapped)<threshold){xMapped=0;}
  if(abs(yMapped)<threshold){yMapped=0;}

  Mouse.move(xMapped, yMapped);

   // Reload logic
  static bool lastReloadState = HIGH;
  bool reloadState = digitalRead(reloadPin);

if (lastReloadState == HIGH && reloadState == LOW) {   // falling edge
  if (magOut) {
    magOut = false;
    ammoCount = maxAmmo;
    Serial.println("Reload Complete");
    digitalWrite(hapticPin, HIGH);
    delay(100);
    digitalWrite(hapticPin, LOW);
    Keyboard.write('r');
  } else {
    magOut = true;
    Serial.println("Magazine Out");
    Keyboard.write('r');
  }
}
lastReloadState = reloadState;
  
  // Fire mode selection
  if (digitalRead(fireSafety) == LOW) { mode = 0; }
  else if (digitalRead(fireSemi) == LOW) { mode = 1; }
  else if (digitalRead(fireBurst) == LOW) { mode = 2; }
  else if (digitalRead(fireAuto) == LOW) { mode = 3; }
  
  bool triggerPressed = (lastTriggerState == HIGH && triggerState == LOW);
  lastTriggerState = triggerState;
  
  if (mode != 0 && !magOut && ammoCount > 0) {
    unsigned long now = millis();
  
    if (mode == 1 && triggerPressed) {
      fireOneShot();
    }
    else if (mode == 2 && triggerPressed) {
      for (int i = 0; i < 3; i++) {
        if (ammoCount > 0 && !magOut) {
          fireOneShot();
          delay(burstDelay);
        } else {
          break;
        }
      }
    }
    else if (mode == 3 && triggerState == LOW) {
      if (now - lastFireTime >= autoFireRate) {
        lastFireTime = now;
        fireOneShot();
      }
    }
  }
  delay(10);
}

void fireOneShot() {
  ammoCount--;
  Serial.print("Shot. Ammo Left: ");
  Serial.println(ammoCount);
  Mouse.click();
  digitalWrite(hapticPin, HIGH);
  delay(100);
  digitalWrite(hapticPin, LOW);
}
