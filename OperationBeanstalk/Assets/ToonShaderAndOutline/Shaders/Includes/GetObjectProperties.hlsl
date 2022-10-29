
#ifndef GET_OBJECT_PROPERTIES_INCLUDED
#define GET_OBJECT_PROPERTIES_INCLUDED

float initX = 0;
float initY = 0;
float initZ = 0;

void SetInitColor_float(float3 XYZ, out float3 position) {
    if (initX == 0 &&
        initY == 0 &&
        initZ == 0) {
        initX = XYZ[0];
        initY = XYZ[1];
        initZ = XYZ[2];
    }

    position = float3(initX, initY, initZ);
}

#endif