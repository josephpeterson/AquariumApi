class Vector {
    constructor(public x, public y, public z) { };
}
export class CameraConfiguration {
    id: number;
    height: number = 1080;
    width: number = 1920;
    sharpness: number = 0;
    contrast: number = 0;
    saturation: number = 0;
    brightness: number = 50;
    exposureMode: string = 'auto';
    iso: number = 100;
    hFlip: boolean = false;
    vFlip: boolean = false;
    roiX: number = 0;
    roiY: number = 0;
    roiW: number = 1;
    roiH: number = 1;
    rotation: number = 0;
}
export var CameraExposureModes = [
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