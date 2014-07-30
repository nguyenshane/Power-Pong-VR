#pragma strict
// This script is a plugin that creates shaking for explosion
var shakeNow: boolean;
var notStopped: boolean = true;
var shakeTime: float;
var shakeDist: float;
var neutralPos: Vector3;
 
function Update () {
    if (shakeNow) {
        shakeNow = false;
        Shake(shakeTime, shakeDist);
    }
}
 
 
function Shake(shakeTime: float, shakeDistance: float) {
    var startTime: float = Time.time;
    neutralPos = transform.position;
   
    while (Time.time < startTime + shakeTime && notStopped) {
        transform.position = neutralPos + Random.insideUnitSphere * shakeDistance;
        yield;
    }
    transform.position = neutralPos;
    notStopped = true;
}