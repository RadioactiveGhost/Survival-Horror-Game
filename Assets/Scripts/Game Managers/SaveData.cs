using UnityEngine;

[System.Serializable]
public class SaveData
{
    //terrain Data
    public float[,,] points;
    public int mapSizeX, mapSizeZ, textureDetailMultiplier, sizeXtile, sizeZtile;
    public float[,,] colors;

    //inventory Data


    public SaveData(TerrainGenerator TG)
    {
        mapSizeX = TG.mapSizeX;
        mapSizeZ = TG.mapSizeZ;
        textureDetailMultiplier = TG.textureDetailMultiplier;
        sizeXtile = TG.sizeXtile;
        sizeZtile = TG.sizeZtile;

        //each 3 point values by tilesize by map size
        points = new float[mapSizeX * mapSizeZ, (TG.sizeXtile + 1) * (TG.sizeZtile + 1), 3];
        colors = new float[mapSizeX * mapSizeZ, (sizeXtile * textureDetailMultiplier) * (sizeZtile * textureDetailMultiplier), 3];

        for (int i = 0; i < mapSizeX * mapSizeZ; i++)
        {
            for (int j = 0; j < points.GetLength(1); j++)
            {
                points[i, j, 0] = TG.map[i].GetComponent<Tile>().vertexes[j].x;
                points[i, j, 1] = TG.map[i].GetComponent<Tile>().vertexes[j].y;
                points[i, j, 2] = TG.map[i].GetComponent<Tile>().vertexes[j].z;
            }

            for (int j = 0; j < colors.GetLength(1); j++)
            {
                colors[i, j, 0] = TG.map[i].GetComponent<Tile>().colors[j].r;
                colors[i, j, 1] = TG.map[i].GetComponent<Tile>().colors[j].g;
                colors[i, j, 2] = TG.map[i].GetComponent<Tile>().colors[j].b;
            }
        }
    }
}