using System;
public readonly struct Coords
{
    public const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3;
    public readonly int x, y;
    public Coords(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Returns the direction b is in relation to this Coords, or -1 if non-adjacent.
    /// </summary>
    /// <returns>Direction from -1 ~ 3.</returns>
    public int AdjacentDir(Coords b)
    {
        if (x == b.x && y + 1 == b.y) return 0;
        if (x == b.x && y - 1 == b.y) return 1;
        if (y == b.y && x - 1 == b.y) return 2;
        if (y == b.y && x + 1 == b.y) return 3;
        return -1;
    }

    /// <summary>
    /// Returns the Coords starting from this Coords and moving one space in the direction dir.
    /// </summary>
    public Coords MoveDir(int dir)
    {
        if (dir == 0) return new Coords(x, y + 1);
        if (dir == 1) return new Coords(x, y - 1);
        if (dir == 2) return new Coords(x - 1, y);
        if (dir == 3) return new Coords(x + 1, y);
        throw new ArgumentOutOfRangeException("Invalid dir in MoveDir: " + dir);
    }
}