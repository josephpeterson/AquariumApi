class Vector {
    constructor(public x, public y, public z) { }
}
export class CameraConfiguration {
    id: number;
    height = 1080;
    width = 1920;
    sharpness = 0;
    contrast = 0;
    saturation = 0;
    brightness = 50;
    exposureMode = 'auto';
    iso = 100;
    hFlip = false;
    vFlip = false;
    roiX = 0;
    roiY = 0;
    roiW = 1;
    roiH = 1;
    rotation = 0;
}
export const CameraExposureModes = [
    { value: "auto", viewValue: "auto" },
    { value: "night", viewValue: "night" },
    { value: "nightpreview", viewValue: "nightpreview" },
    { value: "backlight", viewValue: "backlight" },
    { value: "spotlight", viewValue: "spotlight" },
    { value: "sports", viewValue: "sports" },
    { value: "beach", viewValue: "beach" },
    { value: "verylong", viewValue: "verylong" },
    { value: "fixedfps", viewValue: "fixedfps" },
    { value: "antishake", viewValue: "antishake" },
    { value: "fireworks", viewValue: "fireworks"}
];