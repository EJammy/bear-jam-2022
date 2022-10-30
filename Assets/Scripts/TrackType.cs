using System;
public enum TrackType
{
    NONE = 0,
    HORI,
    VERTI,
    CORNERTL,
    CORNERTR,
    CORNERBL,
    CORNERBR,
    CROSS,
    OBSTACLE,
    STATIONT,
    STATIONB,
    STATIONL,
    STATIONR
}

public static class TrackUtils
{
    public static int stationType(TrackType t) {
        if ((int)t < (int)TrackType.STATIONT) return -1;
        else return ((int)t - (int)TrackType.STATIONT);
    }
}
