// float UVToCenter_float(float val, out float res) {
//     // float y = 0.0;
//     // if (val >== 0.0 && val < 0.1) {
//     //     y = (-45.1 * val) + (-0.0);
//     // } else if (val >= 0.1 && val < 0.2) {
//     //     y = (-19.6 * val) + (-1.51);
//     // } else if (val >= 0.2 && val < 0.3) {
//     //     y = (-11.3 * val) + (-3.47);
//     // } else if (val >= 0.3 && val < 0.4) {
//     //     y = (-7.6 * val) + (-4.8);
//     // } else if (val >= 0.4 && val < 0.5) {
//     //     y = (-5.0 * val) + (-6.16);
//     // } else if (val >= 0.5 && val < 0.6) {
//     //     y = (-3.3 * val) + (-7.66);
//     // } else if (val >= 0.6 && val < 0.7) {
//     //     y = (-2.2 * val) + (-8.99);
//     // } else if (val >= 0.7 && val < 0.8) {
//     //     y = (-1.2 * val) + (-10.07);
//     // } else if (val >= 0.8 && val < 0.9) {
//     //     y = (-0.6 * val) + (-10.87);
//     // } else if (val >= 0.9 && val < 1.0) {
//     //     y = (-0.3 * val) + (-11.46);
//     // } else if (val >== 1.0 && val < 1.1) {
//     //     y = (0.043 * val) + (-11.64);
//     // } else if (val >= 1.1 && val < 1.2) {
//     //     y = (0.082 * val) + (-11.52);
//     // } else if (val >= 1.2 && val < 1.3) {
//     //     y = (0.1175 * val) + (-11.23);
//     // } else if (val >= 1.3 && val < 1.4) {
//     //     y = (0.145 * val) + (-10.8);
//     // } else if (val >= 1.4 && val < 1.5) {
//     //     y = (0.165 * val) + (-10.25);
//     // }
//     res = 1;
// }

void UVToCenter_float(float val, out float newVal, out float res)
{
    if (val >= 0.0 && val < 0.1) {
        res = -100;
        newVal = 0.0;
    } else if (val >= 0.1 && val < 0.2) {
        res = -4.51;
        newVal = 0.1;
    } else if (val >= 0.2 && val < 0.3) {
        res = -1.96;
        newVal = 0.2;
    } else if (val >= 0.3 && val < 0.4) {
        res = -1.13;
        newVal = 0.3;
    } else if (val >= 0.4 && val < 0.5) {
        res = -.76;
        newVal = 0.4;
    } else if (val >= 0.5 && val < 0.6) {
        res = -.5;
        newVal = 0.5;
    } else if (val >= 0.6 && val < 0.7) {
        res = -.33;
        newVal = 0.6;
    } else if (val >= 0.7 && val < 0.8) {
        res = -.22;
        newVal = 0.7;
    } else if (val >= 0.8 && val < 0.9) {
        res = -.12;
        newVal = 0.8;
    } else if (val >= 0.9 && val < 1.0) {
        res = -.06;
        newVal = 0.9;
    } else if (val >= 1.0 && val < 1.1) {
        res = 0;
        newVal = 1;
    } else if (val >= 1.1 && val < 1.2) {
        res = 0.043;
        newVal = 1.1;
    } else if (val >= 1.2 && val < 1.3) {
        res = 0.082;
        newVal = 1.2;
    } else if (val >= 1.3 && val < 1.4) {
        res = 0.1175;
        newVal = 1.3;
    } else if (val >= 1.4 && val < 1.5) {
        res = 0.145;
        newVal = 1.4;
    } else if (val >= 1.5) {
        res = 0.165;
        newVal = 1.5;
    }
}