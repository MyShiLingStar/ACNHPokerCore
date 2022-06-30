using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ACNHPokerCore
{
    public class TerrainUnit
    {
        private byte[] TerrainData;

        public TerrainUnit(byte[] terrainData)
        {
            TerrainData = terrainData;
        }

        public byte[] getTerrainData()
        {
            return TerrainData;
        }

        public string DisplayData()
        {
            byte[] terrainModel = new byte[2];
            Buffer.BlockCopy(TerrainData, 0, terrainModel, 0, 2);
            byte[] terrainVariation = new byte[2];
            Buffer.BlockCopy(TerrainData, 2, terrainVariation, 0, 2);
            byte[] terrainAngle = new byte[2];
            Buffer.BlockCopy(TerrainData, 4, terrainAngle, 0, 2);
            byte[] roadModel = new byte[2];
            Buffer.BlockCopy(TerrainData, 6, roadModel, 0, 2);
            byte[] roadVariation = new byte[2];
            Buffer.BlockCopy(TerrainData, 8, roadVariation, 0, 2);
            byte[] roadAngle = new byte[2];
            Buffer.BlockCopy(TerrainData, 10, roadAngle, 0, 2);
            byte[] elevation = new byte[2];
            Buffer.BlockCopy(TerrainData, 12, elevation, 0, 2);

            return Utilities.flip(Utilities.ByteToHexString(terrainModel)) + " " +
                   Utilities.flip(Utilities.ByteToHexString(terrainVariation)) + " " +
                   Utilities.flip(Utilities.ByteToHexString(terrainAngle)) + " " +
                   Utilities.flip(Utilities.ByteToHexString(roadModel)) + " " +
                   Utilities.flip(Utilities.ByteToHexString(roadVariation)) + " " +
                   Utilities.flip(Utilities.ByteToHexString(roadAngle)) + " " +
                   Utilities.flip(Utilities.ByteToHexString(elevation)) + " " + "\n" +
                   "terrainModel: " + TerrainName[Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(terrainModel)), 16)] + " " + "\n" +
                   "roadModel: " + TerrainName[Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(roadModel)), 16)] + " " + "\n" +
                   "elevation: " + Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(elevation)), 16).ToString();
        }
        public ushort getTerrainModel()
        {
            byte[] terrainModel = new byte[2];
            Buffer.BlockCopy(TerrainData, 0, terrainModel, 0, 2);
            return Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(terrainModel)), 16);
        }
        public ushort getTerrainVariation()
        {
            byte[] terrainVariation = new byte[2];
            Buffer.BlockCopy(TerrainData, 2, terrainVariation, 0, 2);
            return Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(terrainVariation)), 16);
        }
        public ushort getTerrainAngle()
        {
            byte[] terrainAngle = new byte[2];
            Buffer.BlockCopy(TerrainData, 4, terrainAngle, 0, 2);
            return Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(terrainAngle)), 16);
        }
        public ushort getRoadModel()
        {
            byte[] roadModel = new byte[2];
            Buffer.BlockCopy(TerrainData, 6, roadModel, 0, 2);
            return Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(roadModel)), 16);
        }
        public ushort getRoadVariation()
        {
            byte[] roadVariation = new byte[2];
            Buffer.BlockCopy(TerrainData, 8, roadVariation, 0, 2);
            return Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(roadVariation)), 16);
        }
        public ushort getRoadAngle()
        {
            byte[] roadAngle = new byte[2];
            Buffer.BlockCopy(TerrainData, 10, roadAngle, 0, 2);
            return Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(roadAngle)), 16);
        }
        public ushort getElevation()
        {
            byte[] elevation = new byte[2];
            Buffer.BlockCopy(TerrainData, 12, elevation, 0, 2);
            return Convert.ToUInt16(Utilities.flip(Utilities.ByteToHexString(elevation)), 16);
        }

        public bool isSameRoadAndElevation(ushort road, ushort elevation)
        {
            if (elevation != getElevation())
                return false;

            if (road == (ushort)TerrainType.RoadWood)
                return HasRoadWood();
            else if (road == (ushort)TerrainType.RoadTile)
                return HasRoadTile();
            else if (road == (ushort)TerrainType.RoadSand)
                return HasRoadSand();
            else if (road == (ushort)TerrainType.RoadPattern)
                return HasRoadPattern();
            else if (road == (ushort)TerrainType.RoadDarkSoil)
                return HasRoadDarkSoil();
            else if (road == (ushort)TerrainType.RoadBrick)
                return HasRoadBrick();
            else if (road == (ushort)TerrainType.RoadStone)
                return HasRoadStone();
            else if (road == (ushort)TerrainType.RoadSoil)
                return HasRoadSoil();
            else return false;
        }

        public ushort getRoadType()
        {
            if (HasRoadWood())
                return 0;
            else if (HasRoadTile())
                return 1;
            else if (HasRoadSand())
                return 2;
            else if (HasRoadPattern())
                return 3;
            else if (HasRoadDarkSoil())
                return 4;
            else if (HasRoadBrick())
                return 5;
            else if (HasRoadStone())
                return 6;
            else if (HasRoadSoil())
                return 7;
            else
                return 99;
        }

        public bool isSameElevationCliff(ushort elevation)
        {
            if (!isFall() && !isRiver() && elevation == getElevation())
                return true;
            else
                return false;
        }

        public bool isSameOrHigherElevationTerrain(ushort elevation)
        {
            if ((isFall() || isRiver() || isCliff() || isFlat()) && getElevation() >= elevation)
                return true;
            else
                return false;
        }
        public bool isSameOrHigherElevationRiverOrFall(ushort elevation)
        {
            if ((isFall() || isRiver()) && getElevation() >= elevation)
                return true;
            else
                return false;
        }

        public bool isFallOrRiver()
        {
            if (isFall() || isRiver())
                return true;
            else
                return false;
        }

        public bool HasRoad()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadBrick0A || (road >= (ushort)TerrainUnitModel.RoadSoil0A && road <= (ushort)TerrainUnitModel.RoadStone8A))
                return true;
            else
                return false;
        }
        public bool HasTerrain()
        {
            if (isFall() || isCliff() || isRiver() || (isFlat() && getElevation() > 0))
                return true;
            else
                return false;
        }

        public bool HasTerrainVariation(ushort terrain)
        {
            if (terrain == (ushort)TerrainUnitModel.Cliff1A ||
                terrain == (ushort)TerrainUnitModel.Cliff2B ||
                terrain == (ushort)TerrainUnitModel.Cliff2C ||
                terrain == (ushort)TerrainUnitModel.Cliff3A ||
                terrain == (ushort)TerrainUnitModel.Cliff3B ||
                terrain == (ushort)TerrainUnitModel.Cliff3C ||
                terrain == (ushort)TerrainUnitModel.Cliff4A ||
                terrain == (ushort)TerrainUnitModel.Cliff4B ||
                terrain == (ushort)TerrainUnitModel.Cliff5B ||

                terrain == (ushort)TerrainUnitModel.River1A ||
                terrain == (ushort)TerrainUnitModel.River2B ||
                terrain == (ushort)TerrainUnitModel.River2C ||
                terrain == (ushort)TerrainUnitModel.River3A ||
                terrain == (ushort)TerrainUnitModel.River3B ||
                terrain == (ushort)TerrainUnitModel.River3C ||
                terrain == (ushort)TerrainUnitModel.River4A ||
                terrain == (ushort)TerrainUnitModel.River4B ||
                terrain == (ushort)TerrainUnitModel.River5B)
                return true;
            else
                return false;
        }

        public bool HasRoadWood()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadWood0A && road <= (ushort)TerrainUnitModel.RoadWood8A)
                return true;
            else
                return false;
        }
        public bool HasRoadTile()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadTile0A && road <= (ushort)TerrainUnitModel.RoadTile8A)
                return true;
            else
                return false;
        }
        public bool HasRoadSand()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadSand0A && road <= (ushort)TerrainUnitModel.RoadSand8A)
                return true;
            else
                return false;
        }
        public bool HasRoadPattern()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadFanPattern0A && road <= (ushort)TerrainUnitModel.RoadFanPattern8A)
                return true;
            else
                return false;
        }
        public bool HasRoadDarkSoil()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadDarkSoil0A && road <= (ushort)TerrainUnitModel.RoadDarkSoil8A)
                return true;
            else
                return false;
        }
        public bool HasRoadBrick()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadBrick0A && road <= (ushort)TerrainUnitModel.RoadBrick8A)
                return true;
            else
                return false;
        }
        public bool HasRoadStone()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadStone0A && road <= (ushort)TerrainUnitModel.RoadStone8A)
                return true;
            else
                return false;
        }
        public bool HasRoadSoil()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadSoil0A && road <= (ushort)TerrainUnitModel.RoadSoil8A)
                return true;
            else
                return false;
        }
        public bool isFall()
        {
            ushort terrain = getTerrainModel();
            if ((terrain >= (ushort)TerrainUnitModel.Fall101 && terrain <= (ushort)TerrainUnitModel.Fall404) || (terrain >= (ushort)TerrainUnitModel.Fall103 && terrain <= (ushort)TerrainUnitModel.Fall424))
                return true;
            else
                return false;
        }
        public bool isCliff()
        {
            ushort terrain = getTerrainModel();
            if ((terrain >= (ushort)TerrainUnitModel.Cliff0A && terrain <= (ushort)TerrainUnitModel.Cliff8) || (terrain == (ushort)TerrainUnitModel.Cliff2B))
                return true;
            else
                return false;
        }

        public bool isFallCliff()
        {
            ushort terrain = getTerrainModel();
            if (terrain == (ushort)TerrainUnitModel.Cliff5B)
                return true;
            else
                return false;
        }
        public bool isRiver()
        {
            ushort terrain = getTerrainModel();
            if (terrain >= (ushort)TerrainUnitModel.River0A && terrain <= (ushort)TerrainUnitModel.River8A)
                return true;
            else
                return false;
        }

        public bool isFlat()
        {
            ushort terrain = getTerrainModel();
            if (terrain == (ushort)TerrainUnitModel.Base)
                return true;
            else
                return false;
        }

        public bool isRoad0A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil0A ||
                road == (ushort)TerrainUnitModel.RoadStone0A ||
                road == (ushort)TerrainUnitModel.RoadBrick0A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil0A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern0A ||
                road == (ushort)TerrainUnitModel.RoadSand0A ||
                road == (ushort)TerrainUnitModel.RoadTile0A ||
                road == (ushort)TerrainUnitModel.RoadWood0A)
                return true;
            else
                return false;
        }
        public bool isRoad1A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil1A ||
                road == (ushort)TerrainUnitModel.RoadStone1A ||
                road == (ushort)TerrainUnitModel.RoadBrick1A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1A ||
                road == (ushort)TerrainUnitModel.RoadSand1A ||
                road == (ushort)TerrainUnitModel.RoadTile1A ||
                road == (ushort)TerrainUnitModel.RoadWood1A)
                return true;
            else
                return false;
        }
        public bool isRoad0B()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil0B ||
                road == (ushort)TerrainUnitModel.RoadStone0B ||
                road == (ushort)TerrainUnitModel.RoadBrick0B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil0B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern0B ||
                road == (ushort)TerrainUnitModel.RoadSand0B ||
                road == (ushort)TerrainUnitModel.RoadTile0B ||
                road == (ushort)TerrainUnitModel.RoadWood0B)
                return true;
            else
                return false;
        }
        public bool isRoad1B()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil1B ||
                road == (ushort)TerrainUnitModel.RoadStone1B ||
                road == (ushort)TerrainUnitModel.RoadBrick1B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1B ||
                road == (ushort)TerrainUnitModel.RoadSand1B ||
                road == (ushort)TerrainUnitModel.RoadTile1B ||
                road == (ushort)TerrainUnitModel.RoadWood1B)
                return true;
            else
                return false;
        }
        public bool isRoad1C()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil1C ||
                road == (ushort)TerrainUnitModel.RoadStone1C ||
                road == (ushort)TerrainUnitModel.RoadBrick1C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1C ||
                road == (ushort)TerrainUnitModel.RoadSand1C ||
                road == (ushort)TerrainUnitModel.RoadTile1C ||
                road == (ushort)TerrainUnitModel.RoadWood1C)
                return true;
            else
                return false;
        }
        public bool isRoad2A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil2A ||
                road == (ushort)TerrainUnitModel.RoadStone2A ||
                road == (ushort)TerrainUnitModel.RoadBrick2A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil2A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern2A ||
                road == (ushort)TerrainUnitModel.RoadSand2A ||
                road == (ushort)TerrainUnitModel.RoadTile2A ||
                road == (ushort)TerrainUnitModel.RoadWood2A)
                return true;
            else
                return false;
        }
        public bool isRoad2B()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil2B ||
                road == (ushort)TerrainUnitModel.RoadStone2B ||
                road == (ushort)TerrainUnitModel.RoadBrick2B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil2B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern2B ||
                road == (ushort)TerrainUnitModel.RoadSand2B ||
                road == (ushort)TerrainUnitModel.RoadTile2B ||
                road == (ushort)TerrainUnitModel.RoadWood2B)
                return true;
            else
                return false;
        }
        public bool isRoad2C()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil2C ||
                road == (ushort)TerrainUnitModel.RoadStone2C ||
                road == (ushort)TerrainUnitModel.RoadBrick2C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil2C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern2C ||
                road == (ushort)TerrainUnitModel.RoadSand2C ||
                road == (ushort)TerrainUnitModel.RoadTile2C ||
                road == (ushort)TerrainUnitModel.RoadWood2C)
                return true;
            else
                return false;
        }
        public bool isRoad3A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil3A ||
                road == (ushort)TerrainUnitModel.RoadStone3A ||
                road == (ushort)TerrainUnitModel.RoadBrick3A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil3A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern3A ||
                road == (ushort)TerrainUnitModel.RoadSand3A ||
                road == (ushort)TerrainUnitModel.RoadTile3A ||
                road == (ushort)TerrainUnitModel.RoadWood3A)
                return true;
            else
                return false;
        }
        public bool isRoad3B()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil3B ||
                road == (ushort)TerrainUnitModel.RoadStone3B ||
                road == (ushort)TerrainUnitModel.RoadBrick3B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil3B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern3B ||
                road == (ushort)TerrainUnitModel.RoadSand3B ||
                road == (ushort)TerrainUnitModel.RoadTile3B ||
                road == (ushort)TerrainUnitModel.RoadWood3B)
                return true;
            else
                return false;
        }
        public bool isRoad3C()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil3C ||
                road == (ushort)TerrainUnitModel.RoadStone3C ||
                road == (ushort)TerrainUnitModel.RoadBrick3C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil3C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern3C ||
                road == (ushort)TerrainUnitModel.RoadSand3C ||
                road == (ushort)TerrainUnitModel.RoadTile3C ||
                road == (ushort)TerrainUnitModel.RoadWood3C)
                return true;
            else
                return false;
        }
        public bool isRoad4A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil4A ||
                road == (ushort)TerrainUnitModel.RoadStone4A ||
                road == (ushort)TerrainUnitModel.RoadBrick4A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil4A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern4A ||
                road == (ushort)TerrainUnitModel.RoadSand4A ||
                road == (ushort)TerrainUnitModel.RoadTile4A ||
                road == (ushort)TerrainUnitModel.RoadWood4A)
                return true;
            else
                return false;
        }
        public bool isRoad4B()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil4B ||
                road == (ushort)TerrainUnitModel.RoadStone4B ||
                road == (ushort)TerrainUnitModel.RoadBrick4B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil4B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern4B ||
                road == (ushort)TerrainUnitModel.RoadSand4B ||
                road == (ushort)TerrainUnitModel.RoadTile4B ||
                road == (ushort)TerrainUnitModel.RoadWood4B)
                return true;
            else
                return false;
        }
        public bool isRoad4C()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil4C ||
                road == (ushort)TerrainUnitModel.RoadStone4C ||
                road == (ushort)TerrainUnitModel.RoadBrick4C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil4C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern4C ||
                road == (ushort)TerrainUnitModel.RoadSand4C ||
                road == (ushort)TerrainUnitModel.RoadTile4C ||
                road == (ushort)TerrainUnitModel.RoadWood4C)
                return true;
            else
                return false;
        }
        public bool isRoad5A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil5A ||
                road == (ushort)TerrainUnitModel.RoadStone5A ||
                road == (ushort)TerrainUnitModel.RoadBrick5A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil5A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern5A ||
                road == (ushort)TerrainUnitModel.RoadSand5A ||
                road == (ushort)TerrainUnitModel.RoadTile5A ||
                road == (ushort)TerrainUnitModel.RoadWood5A)
                return true;
            else
                return false;
        }
        public bool isRoad5B()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil5B ||
                road == (ushort)TerrainUnitModel.RoadStone5B ||
                road == (ushort)TerrainUnitModel.RoadBrick5B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil5B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern5B ||
                road == (ushort)TerrainUnitModel.RoadSand5B ||
                road == (ushort)TerrainUnitModel.RoadTile5B ||
                road == (ushort)TerrainUnitModel.RoadWood5B)
                return true;
            else
                return false;
        }
        public bool isRoad6A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil6A ||
                road == (ushort)TerrainUnitModel.RoadStone6A ||
                road == (ushort)TerrainUnitModel.RoadBrick6A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil6A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern6A ||
                road == (ushort)TerrainUnitModel.RoadSand6A ||
                road == (ushort)TerrainUnitModel.RoadTile6A ||
                road == (ushort)TerrainUnitModel.RoadWood6A)
                return true;
            else
                return false;
        }
        public bool isRoad6B()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil6B ||
                road == (ushort)TerrainUnitModel.RoadStone6B ||
                road == (ushort)TerrainUnitModel.RoadBrick6B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil6B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern6B ||
                road == (ushort)TerrainUnitModel.RoadSand6B ||
                road == (ushort)TerrainUnitModel.RoadTile6B ||
                road == (ushort)TerrainUnitModel.RoadWood6B)
                return true;
            else
                return false;
        }
        public bool isRoad7A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil7A ||
                road == (ushort)TerrainUnitModel.RoadStone7A ||
                road == (ushort)TerrainUnitModel.RoadBrick7A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil7A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern7A ||
                road == (ushort)TerrainUnitModel.RoadSand7A ||
                road == (ushort)TerrainUnitModel.RoadTile7A ||
                road == (ushort)TerrainUnitModel.RoadWood7A)
                return true;
            else
                return false;
        }
        public bool isRoad8A()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil8A ||
                road == (ushort)TerrainUnitModel.RoadStone8A ||
                road == (ushort)TerrainUnitModel.RoadBrick8A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil8A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern8A ||
                road == (ushort)TerrainUnitModel.RoadSand8A ||
                road == (ushort)TerrainUnitModel.RoadTile8A ||
                road == (ushort)TerrainUnitModel.RoadWood8A)
                return true;
            else
                return false;
        }
        public bool isRoundCornerRoad()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil0B ||
                road == (ushort)TerrainUnitModel.RoadStone0B ||
                road == (ushort)TerrainUnitModel.RoadBrick0B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil0B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern0B ||
                road == (ushort)TerrainUnitModel.RoadSand0B ||
                road == (ushort)TerrainUnitModel.RoadTile0B ||
                road == (ushort)TerrainUnitModel.RoadWood0B ||

                road == (ushort)TerrainUnitModel.RoadSoil1B ||
                road == (ushort)TerrainUnitModel.RoadStone1B ||
                road == (ushort)TerrainUnitModel.RoadBrick1B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1B ||
                road == (ushort)TerrainUnitModel.RoadSand1B ||
                road == (ushort)TerrainUnitModel.RoadTile1B ||
                road == (ushort)TerrainUnitModel.RoadWood1B ||

                road == (ushort)TerrainUnitModel.RoadSoil1C ||
                road == (ushort)TerrainUnitModel.RoadStone1C ||
                road == (ushort)TerrainUnitModel.RoadBrick1C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1C ||
                road == (ushort)TerrainUnitModel.RoadSand1C ||
                road == (ushort)TerrainUnitModel.RoadTile1C ||
                road == (ushort)TerrainUnitModel.RoadWood1C ||

                road == (ushort)TerrainUnitModel.RoadSoil2B ||
                road == (ushort)TerrainUnitModel.RoadStone2B ||
                road == (ushort)TerrainUnitModel.RoadBrick2B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil2B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern2B ||
                road == (ushort)TerrainUnitModel.RoadSand2B ||
                road == (ushort)TerrainUnitModel.RoadTile2B ||
                road == (ushort)TerrainUnitModel.RoadWood2B ||

                road == (ushort)TerrainUnitModel.RoadSoil3B ||
                road == (ushort)TerrainUnitModel.RoadStone3B ||
                road == (ushort)TerrainUnitModel.RoadBrick3B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil3B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern3B ||
                road == (ushort)TerrainUnitModel.RoadSand3B ||
                road == (ushort)TerrainUnitModel.RoadTile3B ||
                road == (ushort)TerrainUnitModel.RoadWood3B
                )
                return true;
            else
                return false;
        }
        public bool canChangeCornerRoad()
        {
            ushort road = getRoadModel();
            if (road == (ushort)TerrainUnitModel.RoadSoil0A ||
                road == (ushort)TerrainUnitModel.RoadStone0A ||
                road == (ushort)TerrainUnitModel.RoadBrick0A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil0A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern0A ||
                road == (ushort)TerrainUnitModel.RoadSand0A ||
                road == (ushort)TerrainUnitModel.RoadTile0A ||
                road == (ushort)TerrainUnitModel.RoadWood0A ||

                road == (ushort)TerrainUnitModel.RoadSoil0B ||
                road == (ushort)TerrainUnitModel.RoadStone0B ||
                road == (ushort)TerrainUnitModel.RoadBrick0B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil0B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern0B ||
                road == (ushort)TerrainUnitModel.RoadSand0B ||
                road == (ushort)TerrainUnitModel.RoadTile0B ||
                road == (ushort)TerrainUnitModel.RoadWood0B ||

                road == (ushort)TerrainUnitModel.RoadSoil1A ||
                road == (ushort)TerrainUnitModel.RoadStone1A ||
                road == (ushort)TerrainUnitModel.RoadBrick1A ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1A ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1A ||
                road == (ushort)TerrainUnitModel.RoadSand1A ||
                road == (ushort)TerrainUnitModel.RoadTile1A ||
                road == (ushort)TerrainUnitModel.RoadWood1A ||

                road == (ushort)TerrainUnitModel.RoadSoil1B ||
                road == (ushort)TerrainUnitModel.RoadStone1B ||
                road == (ushort)TerrainUnitModel.RoadBrick1B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1B ||
                road == (ushort)TerrainUnitModel.RoadSand1B ||
                road == (ushort)TerrainUnitModel.RoadTile1B ||
                road == (ushort)TerrainUnitModel.RoadWood1B ||

                road == (ushort)TerrainUnitModel.RoadSoil1C ||
                road == (ushort)TerrainUnitModel.RoadStone1C ||
                road == (ushort)TerrainUnitModel.RoadBrick1C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil1C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern1C ||
                road == (ushort)TerrainUnitModel.RoadSand1C ||
                road == (ushort)TerrainUnitModel.RoadTile1C ||
                road == (ushort)TerrainUnitModel.RoadWood1C ||

                road == (ushort)TerrainUnitModel.RoadSoil2B ||
                road == (ushort)TerrainUnitModel.RoadStone2B ||
                road == (ushort)TerrainUnitModel.RoadBrick2B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil2B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern2B ||
                road == (ushort)TerrainUnitModel.RoadSand2B ||
                road == (ushort)TerrainUnitModel.RoadTile2B ||
                road == (ushort)TerrainUnitModel.RoadWood2B ||

                road == (ushort)TerrainUnitModel.RoadSoil2C ||
                road == (ushort)TerrainUnitModel.RoadStone2C ||
                road == (ushort)TerrainUnitModel.RoadBrick2C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil2C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern2C ||
                road == (ushort)TerrainUnitModel.RoadSand2C ||
                road == (ushort)TerrainUnitModel.RoadTile2C ||
                road == (ushort)TerrainUnitModel.RoadWood2C ||

                road == (ushort)TerrainUnitModel.RoadSoil3B ||
                road == (ushort)TerrainUnitModel.RoadStone3B ||
                road == (ushort)TerrainUnitModel.RoadBrick3B ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil3B ||
                road == (ushort)TerrainUnitModel.RoadFanPattern3B ||
                road == (ushort)TerrainUnitModel.RoadSand3B ||
                road == (ushort)TerrainUnitModel.RoadTile3B ||
                road == (ushort)TerrainUnitModel.RoadWood3B ||

                road == (ushort)TerrainUnitModel.RoadSoil3C ||
                road == (ushort)TerrainUnitModel.RoadStone3C ||
                road == (ushort)TerrainUnitModel.RoadBrick3C ||
                road == (ushort)TerrainUnitModel.RoadDarkSoil3C ||
                road == (ushort)TerrainUnitModel.RoadFanPattern3C ||
                road == (ushort)TerrainUnitModel.RoadSand3C ||
                road == (ushort)TerrainUnitModel.RoadTile3C ||
                road == (ushort)TerrainUnitModel.RoadWood3C

                )
                return true;
            else
                return false;
        }

        public bool isRoundCornerTerrain()
        {
            ushort terrain = getTerrainModel();
            if (terrain == (ushort)TerrainUnitModel.Cliff2B ||
                terrain == (ushort)TerrainUnitModel.River2B ||

                terrain == (ushort)TerrainUnitModel.Cliff3B ||
                terrain == (ushort)TerrainUnitModel.River3B
                )
                return true;
            else
                return false;
        }

        public bool canChangeCornerCliff()
        {
            ushort terrain = getTerrainModel();
            if (terrain == (ushort)TerrainUnitModel.Cliff2B ||
                terrain == (ushort)TerrainUnitModel.Cliff3B ||
                terrain == (ushort)TerrainUnitModel.Cliff2C ||
                terrain == (ushort)TerrainUnitModel.Cliff3C
                )
                return true;
            else
                return false;
        }

        public bool canChangeCornerRiver()
        {
            ushort terrain = getTerrainModel();
            if (terrain == (ushort)TerrainUnitModel.River2B ||
                terrain == (ushort)TerrainUnitModel.River3B ||
                terrain == (ushort)TerrainUnitModel.River2C ||
                terrain == (ushort)TerrainUnitModel.River3C
                )
                return true;
            else
                return false;
        }

        public Bitmap getImage(int size, int x, int y, bool showRoad, bool showBuilding, bool highlightRoadCorner, bool highlightCliffCorner, bool highlightRiverCorner, bool highlightMouth = false)
        {
            Color borderColor;
            Color backgroundColor = Color.Tomato;
            Color buildingColor = Color.Transparent;
            Color terrainColor = Color.Pink;
            Color roadColor = Color.Purple;

            bool drawBorder = false;
            bool drawBackground = false;
            bool drawBuilding = false;
            bool drawTerrain = false;
            bool drawRoad = false;

            ushort elevation = getElevation();
            /*if (highlightCorner && canChangeCorner())
                borderColor = Color.Crimson;
            else*/
            borderColor = Color.Black;

            drawBorder = true;

            if (showRoad && highlightRoadCorner && canChangeCornerRoad())
            {
                backgroundColor = Color.Violet;
                drawBackground = true;
            }
            else if (isCliff())
            {
                drawBackground = true;
                if (highlightCliffCorner && canChangeCornerCliff())
                {
                    backgroundColor = Color.Violet;
                }
                else if (elevation == 1)
                {
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation0];
                }
                else if (elevation == 2)
                {
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation1];
                }
                else if (elevation >= 3)
                {
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation2];
                }
            }
            else if (isFall() || isRiver())
            {
                drawBackground = true;
                if (highlightRiverCorner && canChangeCornerRiver())
                {
                    backgroundColor = Color.Violet;
                }
                else if (elevation == 0)
                {
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation0];
                }
                else if (elevation == 1)
                {
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation1];
                }
                else if (elevation == 2)
                {
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation2];
                }
                else if (elevation >= 3)
                {
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation3];
                }
            }
            else
            {
                /*if (HasRoad())
                    backgroundColor = TerrainColor[(int)TerrainType.Elevation0];
                else*/
                if (highlightMouth)
                    backgroundColor = Color.Red;
                else
                    backgroundColor = miniMap.GetBackgroundColorLess(x, y);
                drawBackground = true;
            }

            if (showBuilding)
            {
                buildingColor = miniMap.GetBuildingColor(x, y);

                if (buildingColor != Color.Transparent)
                {
                    drawBuilding = true;
                }
            }

            if (isCliff())
            {
                if (highlightRoadCorner && canChangeCornerRoad())
                {
                    drawTerrain = false;
                }
                else
                {
                    drawTerrain = true;
                    if (elevation == 1)
                    {
                        terrainColor = TerrainColor[(int)TerrainType.Elevation1];
                    }
                    else if (elevation == 2)
                    {
                        terrainColor = TerrainColor[(int)TerrainType.Elevation2];
                    }
                    else if (elevation >= 3)
                    {
                        terrainColor = TerrainColor[(int)TerrainType.Elevation3];
                    }
                }
            }
            else if (isFall())
            {
                terrainColor = TerrainColor[(int)TerrainType.Fall];
                drawTerrain = true;
            }
            else if (isRiver())
            {
                terrainColor = TerrainColor[(int)TerrainType.River];
                drawTerrain = true;
            }
            else if (isFlat())
            {
                if (highlightRoadCorner && canChangeCornerRoad())
                {
                    drawTerrain = false;
                }
                else
                {
                    if (elevation == 1)
                    {
                        drawTerrain = true;
                        terrainColor = TerrainColor[(int)TerrainType.Elevation1];
                    }
                    else if (elevation == 2)
                    {
                        drawTerrain = true;
                        terrainColor = TerrainColor[(int)TerrainType.Elevation2];
                    }
                    else if (elevation >= 3)
                    {
                        drawTerrain = true;
                        terrainColor = TerrainColor[(int)TerrainType.Elevation3];
                    }
                }
            }

            if (showRoad)
            {
                if (HasRoad())
                {
                    drawRoad = true;

                    if (HasRoadWood())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadWood];
                    }
                    else if (HasRoadTile())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadTile];
                    }
                    else if (HasRoadSand())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadSand];
                    }
                    else if (HasRoadPattern())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadPattern];
                    }
                    else if (HasRoadDarkSoil())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadDarkSoil];
                    }
                    else if (HasRoadBrick())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadBrick];
                    }
                    else if (HasRoadStone())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadStone];
                    }
                    else if (HasRoadSoil())
                    {
                        roadColor = TerrainColor[(int)TerrainType.RoadSoil];
                    }
                }
            }

            return drawImage(size, borderColor, backgroundColor, buildingColor, terrainColor, roadColor, drawBorder, drawBackground, drawBuilding, drawTerrain, drawRoad);
        }

        public void updateRoad(ushort road, bool[,] neighbour, bool roundCorner = false)
        {
            bool up = neighbour[1, 0];
            bool down = neighbour[1, 2];
            bool left = neighbour[0, 1];
            bool right = neighbour[2, 1];
            bool topleft = neighbour[0, 0];
            bool topright = neighbour[2, 0];
            bool bottomleft = neighbour[0, 2];
            bool bottomright = neighbour[2, 2];

            if (up && down && left && right &&
                topleft && topright && bottomleft && bottomright)
            {
                //Debug.Print("8A");
                setRoad(road, "8A", 0);
            }
            //========================================================
            else if (up && down && left && right &&
                topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("7A 0");
                setRoad(road, "7A", 0);
            }
            else if (up && down && left && right &&
                !topleft && topright && bottomleft && bottomright)
            {
                //Debug.Print("7A 3");
                setRoad(road, "7A", 3);
            }
            else if (up && down && left && right &&
                topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("7A 2");
                setRoad(road, "7A", 2);
            }
            else if (up && down && left && right &&
                topleft && topright && bottomleft && !bottomright)
            {
                //Debug.Print("7A 1");
                setRoad(road, "7A", 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && topright && bottomleft && !bottomright)
            {
                //Debug.Print("6A 0");
                setRoad(road, "6A", 0);
            }
            else if (up && down && left && right &&
                topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("6A 1");
                setRoad(road, "6A", 1);
            }
            //========================================================
            else if (up && down && left && right &&
                topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("6B 0");
                setRoad(road, "6B", 0);
            }
            else if (up && down && left && right &&
                !topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("6B 3");
                setRoad(road, "6B", 3);
            }
            else if (up && down && left && right &&
                !topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("6B 2");
                setRoad(road, "6B", 2);
            }
            else if (up && down && left && right &&
                topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("6B 1");
                setRoad(road, "6B", 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("5A 0");
                setRoad(road, "5A", 0);
            }
            else if (up && down && left && right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5A 3");
                setRoad(road, "5A", 3);
            }
            else if (up && down && left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5A 2");
                setRoad(road, "5A", 2);
            }
            else if (up && down && left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("5A 1");
                setRoad(road, "5A", 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("5B 0");
                setRoad(road, "5B", 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("5B 3");
                setRoad(road, "5B", 3);
            }
            else if (up && down && left && !right &&
                topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("5B 2");
                setRoad(road, "5B", 2);
            }
            else if (up && !down && left && right &&
                topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5B 1");
                setRoad(road, "5B", 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("4A 0");
                setRoad(road, "4A", 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("4A 3");
                setRoad(road, "4A", 3);
            }
            else if (up && down && left && !right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4A 2");
                setRoad(road, "4A", 2);
            }
            else if (up && !down && left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4A 1");
                setRoad(road, "4A", 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4B 0");
                setRoad(road, "4B", 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("4B 3");
                setRoad(road, "4B", 3);
            }
            else if (up && down && left && !right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("4B 2");
                setRoad(road, "4B", 2);
            }
            else if (up && !down && left && right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4B 1");
                setRoad(road, "4B", 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4C");
                setRoad(road, "4C", 0);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 0");
                setRoad(road, "3A", 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 3");
                setRoad(road, "3A", 3);
            }
            else if (up && down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 2");
                setRoad(road, "3A", 2);
            }
            else if (up && !down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 1");
                setRoad(road, "3A", 1);
            }
            //========================================================
            else if (!up && down && !left && right &&
                    !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("3B/C 0");
                if (roundCorner)
                    setRoad(road, "3B", 0);
                else
                    setRoad(road, "3C", 0);
            }
            else if (!up && down && left && !right &&
                    !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 3");
                if (roundCorner)
                    setRoad(road, "3B", 3);
                else
                    setRoad(road, "3C", 3);
            }
            else if (up && !down && left && !right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 2");
                if (roundCorner)
                    setRoad(road, "3B", 2);
                else
                    setRoad(road, "3C", 2);
            }
            else if (up && !down && !left && right &&
                    !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 1");
                if (roundCorner)
                    setRoad(road, "3B", 1);
                else
                    setRoad(road, "3C", 1);
            }
            //========================================================
            else if (up && down && !left && !right &&
                    !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2A 0");
                setRoad(road, "2A", 0);
            }
            else if (!up && !down && left && right &&
                    !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2A 1");
                setRoad(road, "2A", 1);
            }
            //========================================================
            else if (!up && down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 0");
                if (roundCorner)
                    setRoad(road, "2B", 0);
                else
                    setRoad(road, "2C", 0);
            }
            else if (!up && down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 3");
                if (roundCorner)
                    setRoad(road, "2B", 3);
                else
                    setRoad(road, "2C", 3);
            }
            else if (up && !down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 2");
                if (roundCorner)
                    setRoad(road, "2B", 2);
                else
                    setRoad(road, "2C", 2);
            }
            else if (up && !down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 1");
                if (roundCorner)
                    setRoad(road, "2B", 1);
                else
                    setRoad(road, "2C", 1);
            }
            //========================================================
            else if (!up && down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A/B/C 0");
                if (roundCorner)
                {
                    if (isRoad0B())
                    {
                        if (getRoadAngle() == 0)
                            setRoad(road, "1B", 0);
                        else if (getRoadAngle() == 1)
                            setRoad(road, "1A", 0);
                        else if (getRoadAngle() == 2)
                            setRoad(road, "1A", 0);
                        else if (getRoadAngle() == 3)
                            setRoad(road, "1C", 0);
                        else
                            setRoad(road, "1A", 0);
                    }
                    if (isRoad1B())
                        setRoad(road, "1B", 0);
                    else if (isRoad1C())
                        setRoad(road, "1C", 0);
                    else if (isRoad2B() || isRoad3B())
                    {
                        var currentAngle = getRoadAngle();
                        if (currentAngle == 0)
                            setRoad(road, "1B", 0);
                        else if (currentAngle == 3)
                            setRoad(road, "1C", 0);
                        else
                            setRoad(road, "1A", 0);
                    }
                    else
                        setRoad(road, "1A", 0);
                }
                else
                    setRoad(road, "1A", 0);
            }
            else if (!up && !down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A/B/C 3");
                if (roundCorner)
                {
                    if (isRoad0B())
                    {
                        if (getRoadAngle() == 0)
                            setRoad(road, "1A", 3);
                        else if (getRoadAngle() == 1)
                            setRoad(road, "1A", 3);
                        else if (getRoadAngle() == 2)
                            setRoad(road, "1C", 3);
                        else if (getRoadAngle() == 3)
                            setRoad(road, "1B", 3);
                        else
                            setRoad(road, "1A", 3);
                    }
                    else if (isRoad1B())
                        setRoad(road, "1B", 3);
                    else if (isRoad1C())
                        setRoad(road, "1C", 3);
                    else if (isRoad2B() || isRoad3B())
                    {
                        var currentAngle = getRoadAngle();
                        if (currentAngle == 3)
                            setRoad(road, "1B", 3);
                        else if (currentAngle == 2)
                            setRoad(road, "1C", 3);
                        else
                            setRoad(road, "1A", 3);
                    }
                    else
                        setRoad(road, "1A", 3);
                }
                else
                    setRoad(road, "1A", 3);
            }
            else if (up && !down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A/B/C 2");
                if (roundCorner)
                {
                    if (isRoad0B())
                    {
                        if (getRoadAngle() == 0)
                            setRoad(road, "1A", 2);
                        else if (getRoadAngle() == 1)
                            setRoad(road, "1C", 2);
                        else if (getRoadAngle() == 2)
                            setRoad(road, "1B", 2);
                        else if (getRoadAngle() == 3)
                            setRoad(road, "1A", 2);
                        else
                            setRoad(road, "1A", 2);
                    }
                    else if (isRoad1B())
                        setRoad(road, "1B", 2);
                    else if (isRoad1C())
                        setRoad(road, "1C", 2);
                    else if (isRoad2B() || isRoad3B())
                    {
                        var currentAngle = getRoadAngle();
                        if (currentAngle == 1)
                            setRoad(road, "1C", 2);
                        else if (currentAngle == 2)
                            setRoad(road, "1B", 2);
                        else
                            setRoad(road, "1A", 0);
                    }
                    else
                        setRoad(road, "1A", 2);
                }
                else
                    setRoad(road, "1A", 2);
            }
            else if (!up && !down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A/B/C 1");
                if (roundCorner)
                {
                    if (isRoad0B())
                    {
                        if (getRoadAngle() == 0)
                            setRoad(road, "1C", 1);
                        else if (getRoadAngle() == 1)
                            setRoad(road, "1B", 1);
                        else if (getRoadAngle() == 2)
                            setRoad(road, "1A", 1);
                        else if (getRoadAngle() == 3)
                            setRoad(road, "1A", 1);
                        else
                            setRoad(road, "1A", 1);
                    }
                    else if (isRoad1B())
                        setRoad(road, "1B", 1);
                    else if (isRoad1C())
                        setRoad(road, "1C", 1);
                    else if (isRoad2B() || isRoad3B())
                    {
                        var currentAngle = getRoadAngle();
                        if (currentAngle == 0)
                            setRoad(road, "1C", 1);
                        else if (currentAngle == 1)
                            setRoad(road, "1B", 1);
                        else
                            setRoad(road, "1A", 0);
                    }
                    else
                        setRoad(road, "1A", 1);
                }
                else
                    setRoad(road, "1A", 1);
            }
            //========================================================
            else if (!up && !down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("0A/B");
                if (roundCorner)
                {
                    if (isRoad1B())
                        setRoad(road, "0B", getRoadAngle());
                    else if (isRoad1C())
                    {
                        var correctAngle = getRoadAngle() - 1;
                        if (correctAngle < 0)
                            correctAngle = 3;
                        setRoad(road, "0B", (ushort)correctAngle);
                    }
                    else
                        setRoad(road, "0B", getRoadAngle());
                }
                else
                    setRoad(road, "0A", 0);
            }
            else
            {
                MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void setRoad(ushort road, string type, ushort direction)
        {
            ushort writeValue = 0;

            if (road == 0)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadWood0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadWood1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadWood1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadWood2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadWood2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadWood3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadWood3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadWood4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadWood4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadWood5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadWood6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadWood8A;
                        break;
                }
            }
            else if (road == 1)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadTile0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadTile1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadTile1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadTile2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadTile2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadTile3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadTile3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadTile4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadTile4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadTile5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadTile6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadTile8A;
                        break;
                }
            }
            else if (road == 2)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadSand0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadSand1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadSand1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadSand2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadSand2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadSand3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadSand3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadSand4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadSand4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadSand5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadSand6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadSand8A;
                        break;
                }
            }
            else if (road == 3)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadFanPattern8A;
                        break;
                }
            }
            else if (road == 4)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadDarkSoil8A;
                        break;
                }
            }
            else if (road == 5)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadBrick8A;
                        break;
                }
            }
            else if (road == 6)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadStone0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadStone1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadStone1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadStone2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadStone2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadStone3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadStone3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadStone4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadStone4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadStone5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadStone6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadStone8A;
                        break;
                }
            }
            else if (road == 7)
            {
                switch (type)
                {
                    case "0A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil0A;
                        break;
                    case "0B":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil0B;
                        break;
                    case "1A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil1A;
                        break;
                    case "1B":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil1B;
                        break;
                    case "1C":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil1C;
                        break;
                    case "2A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil2A;
                        break;
                    case "2B":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil2B;
                        break;
                    case "2C":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil2C;
                        break;
                    case "3A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil3A;
                        break;
                    case "3B":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil3B;
                        break;
                    case "3C":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil3C;
                        break;
                    case "4A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil4A;
                        break;
                    case "4B":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil4B;
                        break;
                    case "4C":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil4C;
                        break;
                    case "5A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil5A;
                        break;
                    case "5B":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil5B;
                        break;
                    case "6A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil6A;
                        break;
                    case "6B":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil6B;
                        break;
                    case "7A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil7A;
                        break;
                    case "8A":
                        writeValue = (ushort)TerrainUnitModel.RoadSoil8A;
                        break;
                }
            }
            else if (road == 99)
            {
                writeValue = (ushort)TerrainUnitModel.Base;
            }

            setRoadModel(writeValue);
            setRoadAngle(direction);
        }

        public void updateCliff(bool[,] neighbour, ushort elevation, bool roundCorner = false)
        {
            bool up = neighbour[1, 0];
            bool down = neighbour[1, 2];
            bool left = neighbour[0, 1];
            bool right = neighbour[2, 1];
            bool topleft = neighbour[0, 0];
            bool topright = neighbour[2, 0];
            bool bottomleft = neighbour[0, 2];
            bool bottomright = neighbour[2, 2];

            if (up && down && left && right &&
                topleft && topright && bottomleft && bottomright)
            {
                //Debug.Print("8A");
                setCliff("8", elevation, 0);
            }
            //========================================================
            else if (up && down && left && right &&
                topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("7A 0");
                setCliff("7A", elevation, 0);
            }
            else if (up && down && left && right &&
                !topleft && topright && bottomleft && bottomright)
            {
                //Debug.Print("7A 3");
                setCliff("7A", elevation, 3);
            }
            else if (up && down && left && right &&
                topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("7A 2");
                setCliff("7A", elevation, 2);
            }
            else if (up && down && left && right &&
                topleft && topright && bottomleft && !bottomright)
            {
                //Debug.Print("7A 1");
                setCliff("7A", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && topright && bottomleft && !bottomright)
            {
                //Debug.Print("6A 0");
                setCliff("6A", elevation, 0);
            }
            else if (up && down && left && right &&
                topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("6A 1");
                setCliff("6A", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("6B 0");
                setCliff("6B", elevation, 0);
            }
            else if (up && down && left && right &&
                !topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("6B 3");
                setCliff("6B", elevation, 3);
            }
            else if (up && down && left && right &&
                !topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("6B 2");
                setCliff("6B", elevation, 2);
            }
            else if (up && down && left && right &&
                topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("6B 1");
                setCliff("6B", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("5A 0");
                setCliff("5A", elevation, 0);
            }
            else if (up && down && left && right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5A 3");
                setCliff("5A", elevation, 3);
            }
            else if (up && down && left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5A 2");
                setCliff("5A", elevation, 2);
            }
            else if (up && down && left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("5A 1");
                setCliff("5A", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("5B 0");
                setCliff("5B", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("5B 3");
                setCliff("5B", elevation, 3);
            }
            else if (up && down && left && !right &&
                topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("5B 2");
                setCliff("5B", elevation, 2);
            }
            else if (up && !down && left && right &&
                topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5B 1");
                setCliff("5B", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("4A 0");
                setCliff("4A", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("4A 3");
                setCliff("4A", elevation, 3);
            }
            else if (up && down && left && !right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4A 2");
                setCliff("4A", elevation, 2);
            }
            else if (up && !down && left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4A 1");
                setCliff("4A", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4B 0");
                setCliff("4B", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("4B 3");
                setCliff("4B", elevation, 3);
            }
            else if (up && down && left && !right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("4B 2");
                setCliff("4B", elevation, 2);
            }
            else if (up && !down && left && right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4B 1");
                setCliff("4B", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4C");
                setCliff("4C", elevation, 0);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 0");
                setCliff("3A", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 3");
                setCliff("3A", elevation, 3);
            }
            else if (up && down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 2");
                setCliff("3A", elevation, 2);
            }
            else if (up && !down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 1");
                setCliff("3A", elevation, 1);
            }
            //========================================================
            else if (!up && down && !left && right &&
                    !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("3B/C 0");
                if (roundCorner)
                    setCliff("3B", elevation, 0);
                else
                    setCliff("3C", elevation, 0);
            }
            else if (!up && down && left && !right &&
                    !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 3");
                if (roundCorner)
                    setCliff("3B", elevation, 3);
                else
                    setCliff("3C", elevation, 3);
            }
            else if (up && !down && left && !right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 2");
                if (roundCorner)
                    setCliff("3B", elevation, 2);
                else
                    setCliff("3C", elevation, 2);
            }
            else if (up && !down && !left && right &&
                    !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 1");
                if (roundCorner)
                    setCliff("3B", elevation, 1);
                else
                    setCliff("3C", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && !right &&
                    !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2A 0");
                setCliff("2A", elevation, 0);
            }
            else if (!up && !down && left && right &&
                    !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2A 1");
                setCliff("2A", elevation, 1);
            }
            //========================================================
            else if (!up && down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 0");
                if (roundCorner)
                    setCliff("2B", elevation, 0);
                else
                    setCliff("2C", elevation, 0);
            }
            else if (!up && down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 3");
                if (roundCorner)
                    setCliff("2B", elevation, 3);
                else
                    setCliff("2C", elevation, 3);
            }
            else if (up && !down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 2");
                if (roundCorner)
                    setCliff("2B", elevation, 2);
                else
                    setCliff("2C", elevation, 2);
            }
            else if (up && !down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 1");
                if (roundCorner)
                    setCliff("2B", elevation, 1);
                else
                    setCliff("2C", elevation, 1);
            }
            //========================================================
            else if (!up && down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 0");
                setCliff("1A", elevation, 0);
            }
            else if (!up && !down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 3");
                setCliff("1A", elevation, 3);
            }
            else if (up && !down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 2");
                setCliff("1A", elevation, 2);
            }
            else if (!up && !down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 1");
                setCliff("1A", elevation, 1);
            }
            //========================================================
            else if (!up && !down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                setCliff("0A", elevation, 0);
            }
            else
            {
                MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void setCliff(string type, ushort elevation, ushort direction)
        {
            ushort writeValue = 0;

            switch (type)
            {
                case "0A":
                    writeValue = (ushort)TerrainUnitModel.Cliff0A;
                    break;
                case "1A":
                    writeValue = (ushort)TerrainUnitModel.Cliff1A;
                    break;
                case "2A":
                    writeValue = (ushort)TerrainUnitModel.Cliff2A;
                    break;
                case "2B":
                    writeValue = (ushort)TerrainUnitModel.Cliff2B;
                    break;
                case "2C":
                    writeValue = (ushort)TerrainUnitModel.Cliff2C;
                    break;
                case "3A":
                    writeValue = (ushort)TerrainUnitModel.Cliff3A;
                    break;
                case "3B":
                    writeValue = (ushort)TerrainUnitModel.Cliff3B;
                    break;
                case "3C":
                    writeValue = (ushort)TerrainUnitModel.Cliff3C;
                    break;
                case "4A":
                    writeValue = (ushort)TerrainUnitModel.Cliff4A;
                    break;
                case "4B":
                    writeValue = (ushort)TerrainUnitModel.Cliff4B;
                    break;
                case "4C":
                    writeValue = (ushort)TerrainUnitModel.Cliff4C;
                    break;
                case "5A":
                    writeValue = (ushort)TerrainUnitModel.Cliff5A;
                    break;
                case "5B":
                    writeValue = (ushort)TerrainUnitModel.Cliff5B;
                    break;
                case "6A":
                    writeValue = (ushort)TerrainUnitModel.Cliff6A;
                    break;
                case "6B":
                    writeValue = (ushort)TerrainUnitModel.Cliff6B;
                    break;
                case "7A":
                    writeValue = (ushort)TerrainUnitModel.Cliff7A;
                    break;
                case "8":
                    writeValue = (ushort)TerrainUnitModel.Base;
                    break;
            }

            setTerrainModel(writeValue);
            setTerrainAngle(direction);
            setTerrainElevation(elevation);

            if (HasTerrainVariation(writeValue))
            {
                Random rnd = new Random();
                ushort newVariation = (ushort)rnd.Next(0, 3);
                setTerrainVariation(newVariation);
            }
            else
            {
                setTerrainVariation(0);
            }
        }

        public void updateRiver(bool[,] neighbour, ushort elevation, bool roundCorner = false)
        {
            bool up = neighbour[1, 0];
            bool down = neighbour[1, 2];
            bool left = neighbour[0, 1];
            bool right = neighbour[2, 1];
            bool topleft = neighbour[0, 0];
            bool topright = neighbour[2, 0];
            bool bottomleft = neighbour[0, 2];
            bool bottomright = neighbour[2, 2];

            if (up && down && left && right &&
                topleft && topright && bottomleft && bottomright)
            {
                //Debug.Print("8A");
                setRiver("8A", elevation, 0);
            }
            //========================================================
            else if (up && down && left && right &&
                topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("7A 0");
                setRiver("7A", elevation, 0);
            }
            else if (up && down && left && right &&
                !topleft && topright && bottomleft && bottomright)
            {
                //Debug.Print("7A 3");
                setRiver("7A", elevation, 3);
            }
            else if (up && down && left && right &&
                topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("7A 2");
                setRiver("7A", elevation, 2);
            }
            else if (up && down && left && right &&
                topleft && topright && bottomleft && !bottomright)
            {
                //Debug.Print("7A 1");
                setRiver("7A", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && topright && bottomleft && !bottomright)
            {
                //Debug.Print("6A 0");
                setRiver("6A", elevation, 0);
            }
            else if (up && down && left && right &&
                topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("6A 1");
                setRiver("6A", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("6B 0");
                setRiver("6B", elevation, 0);
            }
            else if (up && down && left && right &&
                !topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("6B 3");
                setRiver("6B", elevation, 3);
            }
            else if (up && down && left && right &&
                !topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("6B 2");
                setRiver("6B", elevation, 2);
            }
            else if (up && down && left && right &&
                topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("6B 1");
                setRiver("6B", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("5A 0");
                setRiver("5A", elevation, 0);
            }
            else if (up && down && left && right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5A 3");
                setRiver("5A", elevation, 3);
            }
            else if (up && down && left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5A 2");
                setRiver("5A", elevation, 2);
            }
            else if (up && down && left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("5A 1");
                setRiver("5A", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && topright && !bottomleft && bottomright)
            {
                //Debug.Print("5B 0");
                setRiver("5B", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && bottomleft && bottomright)
            {
                //Debug.Print("5B 3");
                setRiver("5B", elevation, 3);
            }
            else if (up && down && left && !right &&
                topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("5B 2");
                setRiver("5B", elevation, 2);
            }
            else if (up && !down && left && right &&
                topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("5B 1");
                setRiver("5B", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("4A 0");
                setRiver("4A", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("4A 3");
                setRiver("4A", elevation, 3);
            }
            else if (up && down && left && !right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4A 2");
                setRiver("4A", elevation, 2);
            }
            else if (up && !down && left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4A 1");
                setRiver("4A", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4B 0");
                setRiver("4B", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("4B 3");
                setRiver("4B", elevation, 3);
            }
            else if (up && down && left && !right &&
                !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("4B 2");
                setRiver("4B", elevation, 2);
            }
            else if (up && !down && left && right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4B 1");
                setRiver("4B", elevation, 1);
            }
            //========================================================
            else if (up && down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("4C");
                setRiver("4C", elevation, 0);
            }
            //========================================================
            else if (up && down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 0");
                setRiver("3A", elevation, 0);
            }
            else if (!up && down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 3");
                setRiver("3A", elevation, 3);
            }
            else if (up && down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 2");
                setRiver("3A", elevation, 2);
            }
            else if (up && !down && left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3A 1");
                setRiver("3A", elevation, 1);
            }
            //========================================================
            else if (!up && down && !left && right &&
                    !topleft && !topright && !bottomleft && bottomright)
            {
                //Debug.Print("3B/C 0");
                if (roundCorner)
                    setRiver("3B", elevation, 0);
                else
                    setRiver("3C", elevation, 0);
            }
            else if (!up && down && left && !right &&
                    !topleft && !topright && bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 3");
                if (roundCorner)
                    setRiver("3B", elevation, 3);
                else
                    setRiver("3C", elevation, 3);
            }
            else if (up && !down && left && !right &&
                topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 2");
                if (roundCorner)
                    setRiver("3B", elevation, 2);
                else
                    setRiver("3C", elevation, 2);
            }
            else if (up && !down && !left && right &&
                    !topleft && topright && !bottomleft && !bottomright)
            {
                //Debug.Print("3B/C 1");
                if (roundCorner)
                    setRiver("3B", elevation, 1);
                else
                    setRiver("3C", elevation, 1);
            }
            //========================================================
            else if (up && down && !left && !right &&
                    !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2A 0");
                setRiver("2A", elevation, 0);
            }
            else if (!up && !down && left && right &&
                    !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2A 1");
                setRiver("2A", elevation, 1);
            }
            //========================================================
            else if (!up && down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 0");
                if (roundCorner)
                    setRiver("2B", elevation, 0);
                else
                    setRiver("2C", elevation, 0);
            }
            else if (!up && down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 3");
                if (roundCorner)
                    setRiver("2B", elevation, 3);
                else
                    setRiver("2C", elevation, 3);
            }
            else if (up && !down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 2");
                if (roundCorner)
                    setRiver("2B", elevation, 2);
                else
                    setRiver("2C", elevation, 2);
            }
            else if (up && !down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("2B/C 1");
                if (roundCorner)
                    setRiver("2B", elevation, 1);
                else
                    setRiver("2C", elevation, 1);
            }
            //========================================================
            else if (!up && down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 0");
                setRiver("1A", elevation, 0);
            }
            else if (!up && !down && left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 3");
                setRiver("1A", elevation, 3);
            }
            else if (up && !down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 2");
                setRiver("1A", elevation, 2);
            }
            else if (!up && !down && !left && right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                //Debug.Print("1A 1");
                setRiver("1A", elevation, 1);
            }
            //========================================================
            else if (!up && !down && !left && !right &&
                !topleft && !topright && !bottomleft && !bottomright)
            {
                setRiver("0A", elevation, 0);
            }
            else
            {
                MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void setRiver(string type, ushort elevation, ushort direction)
        {
            ushort writeValue = 0;

            switch (type)
            {
                case "0A":
                    writeValue = (ushort)TerrainUnitModel.River0A;
                    break;
                case "1A":
                    writeValue = (ushort)TerrainUnitModel.River1A;
                    break;
                case "2A":
                    writeValue = (ushort)TerrainUnitModel.River2A;
                    break;
                case "2B":
                    writeValue = (ushort)TerrainUnitModel.River2B;
                    break;
                case "2C":
                    writeValue = (ushort)TerrainUnitModel.River2C;
                    break;
                case "3A":
                    writeValue = (ushort)TerrainUnitModel.River3A;
                    break;
                case "3B":
                    writeValue = (ushort)TerrainUnitModel.River3B;
                    break;
                case "3C":
                    writeValue = (ushort)TerrainUnitModel.River3C;
                    break;
                case "4A":
                    writeValue = (ushort)TerrainUnitModel.River4A;
                    break;
                case "4B":
                    writeValue = (ushort)TerrainUnitModel.River4B;
                    break;
                case "4C":
                    writeValue = (ushort)TerrainUnitModel.River4C;
                    break;
                case "5A":
                    writeValue = (ushort)TerrainUnitModel.River5A;
                    break;
                case "5B":
                    writeValue = (ushort)TerrainUnitModel.River5B;
                    break;
                case "6A":
                    writeValue = (ushort)TerrainUnitModel.River6A;
                    break;
                case "6B":
                    writeValue = (ushort)TerrainUnitModel.River6B;
                    break;
                case "7A":
                    writeValue = (ushort)TerrainUnitModel.River7A;
                    break;
                case "8A":
                    writeValue = (ushort)TerrainUnitModel.River8A;
                    break;
            }

            setTerrainModel(writeValue);
            setTerrainAngle(direction);
            setTerrainElevation(elevation);

            if (HasTerrainVariation(writeValue))
            {
                Random rnd = new Random();
                ushort newVariation = (ushort)rnd.Next(0, 3);
                setTerrainVariation(newVariation);
            }
            else
            {
                setTerrainVariation(0);
            }
        }

        public void updateFall(bool[,] ConnectNeighbour, ushort CliffDirection, ushort elevation)
        {
            bool[,] CorrectConnectNeighbour;
            ushort CorrectDirection;
            if (CliffDirection == 0) //LEFT
            {
                CorrectConnectNeighbour = RotateMatrix(ConnectNeighbour);
                CorrectDirection = 3;
            }
            else if (CliffDirection == 1) //DOWN
            {
                CorrectConnectNeighbour = ConnectNeighbour;
                CorrectDirection = 0;
            }
            else if (CliffDirection == 2) //Right
            {
                CorrectConnectNeighbour = RotateMatrix(RotateMatrix(RotateMatrix(ConnectNeighbour)));
                CorrectDirection = 1;
            }
            else //UP
            {
                CorrectConnectNeighbour = RotateMatrix(RotateMatrix(ConnectNeighbour));
                CorrectDirection = 2;
            }

            bool up = CorrectConnectNeighbour[1, 0];
            bool down = CorrectConnectNeighbour[1, 2];
            bool left = CorrectConnectNeighbour[0, 1];
            bool right = CorrectConnectNeighbour[2, 1];
            bool topleft = CorrectConnectNeighbour[0, 0];
            bool topright = CorrectConnectNeighbour[2, 0];
            bool bottomleft = CorrectConnectNeighbour[0, 2];
            bool bottomright = CorrectConnectNeighbour[2, 2];

            if (!left && !right) //10x
            {
                if (up && down)
                {
                    //Debug.Print("100");
                    setFall("100", CorrectDirection);
                }
                else if (up && !down)
                {
                    //Debug.Print("101");
                    setFall("101", CorrectDirection);
                }
                else if (!up && down)
                {
                    //Debug.Print("102");
                    setFall("102", CorrectDirection);
                }
                else if (!up && !down)
                {
                    //Debug.Print("103");
                    setFall("103", CorrectDirection);
                }
                else
                {
                    MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (left && !right) //20x
            {
                if (topleft && up &&
                    bottomleft && down)
                {
                    //Debug.Print("200");
                    setFall("200", CorrectDirection);
                }
                else if (topleft && up &&
                         !bottomleft && down)
                {
                    //Debug.Print("201");
                    setFall("201", CorrectDirection);
                }
                else if (topleft && up &&
                         !bottomleft && !down)
                {
                    //Debug.Print("202");
                    setFall("202", CorrectDirection);
                }
                else if (!topleft && up &&
                         bottomleft && down)
                {
                    //Debug.Print("203");
                    setFall("203", CorrectDirection);
                }
                else if (!topleft && !up &&
                         bottomleft && down)
                {
                    //Debug.Print("204");
                    setFall("204", CorrectDirection);
                }
                else if (!topleft && up &&
                        !bottomleft && down)
                {
                    //Debug.Print("205");
                    setFall("205", CorrectDirection);
                }
                else if (!topleft && !up &&
                         !bottomleft && down)
                {
                    //Debug.Print("206");
                    setFall("206", CorrectDirection);
                }
                else if (!topleft && up &&
                         !bottomleft && !down)
                {
                    //Debug.Print("207");
                    setFall("207", CorrectDirection);
                }
                else if (!topleft && !up &&
                         !bottomleft && !down)
                {
                    //Debug.Print("208");
                    setFall("208", CorrectDirection);
                }
                else
                {
                    MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (!left && right) //30x
            {
                if (up && topright &&
                    down && bottomright)
                {
                    //Debug.Print("300");
                    setFall("300", CorrectDirection);
                }
                else if (up && topright &&
                        down && !bottomright)
                {
                    //Debug.Print("301");
                    setFall("301", CorrectDirection);
                }
                else if (up && topright &&
                        !down && !bottomright)
                {
                    //Debug.Print("302");
                    setFall("302", CorrectDirection);
                }
                else if (up && !topright &&
                        down && bottomright)
                {
                    //Debug.Print("303");
                    setFall("303", CorrectDirection);
                }
                else if (!up && !topright &&
                        down && bottomright)
                {
                    //Debug.Print("304");
                    setFall("304", CorrectDirection);
                }
                else if (up && !topright &&
                        down && !bottomright)
                {
                    //Debug.Print("305");
                    setFall("305", CorrectDirection);
                }
                else if (!up && !topright &&
                        down && !bottomright)
                {
                    //Debug.Print("306");
                    setFall("306", CorrectDirection);
                }
                else if (up && !topright &&
                        !down && !bottomright)
                {
                    //Debug.Print("307");
                    setFall("307", CorrectDirection);
                }
                else if (!up && !topright &&
                        !down && !bottomright)
                {
                    //Debug.Print("308");
                    setFall("308", CorrectDirection);
                }
                else
                {
                    MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (right && left) //40x
            {
                if (topleft && up && topright &&
                    bottomleft && down && bottomright)
                {
                    //Debug.Print("400");
                    setFall("400", CorrectDirection);
                }
                else if (topleft && up && topright &&
                    bottomleft && down && !bottomright)
                {
                    //Debug.Print("401");
                    setFall("401", CorrectDirection);
                }
                else if (topleft && up && topright &&
                    !bottomleft && down && bottomright)
                {
                    //Debug.Print("402");
                    setFall("402", CorrectDirection);
                }
                else if (topleft && up && topright &&
                    !bottomleft && !down && !bottomright)
                {
                    //Debug.Print("403");
                    setFall("403", CorrectDirection);
                }
                else if (topleft && up && topright &&
                    !bottomleft && down && !bottomright)
                {
                    //Debug.Print("404");
                    setFall("404", CorrectDirection);
                }
                else if (topleft && up && !topright &&
                    bottomleft && down && bottomright)
                {
                    //Debug.Print("405");
                    setFall("405", CorrectDirection);
                }
                else if (!topleft && up && topright &&
                    bottomleft && down && bottomright)
                {
                    //Debug.Print("406");
                    setFall("406", CorrectDirection);
                }
                else if (!topleft && up && !topright &&
                    bottomleft && down && bottomright)
                {
                    //Debug.Print("407");
                    setFall("407", CorrectDirection);
                }
                else if (!topleft && !up && !topright &&
                    bottomleft && down && bottomright)
                {
                    //Debug.Print("408");
                    setFall("408", CorrectDirection);
                }
                else if (!topleft && up && topright &&
                    !bottomleft && down && bottomright)
                {
                    //Debug.Print("409");
                    setFall("409", CorrectDirection);
                }
                else if (topleft && up && !topright &&
                    !bottomleft && down && bottomright)
                {
                    //Debug.Print("410");
                    setFall("410", CorrectDirection);
                }
                else if (!topleft && up && !topright &&
                    !bottomleft && down && bottomright)
                {
                    //Debug.Print("411");
                    setFall("411", CorrectDirection);
                }
                else if (!topleft && !up && !topright &&
                    !bottomleft && down && bottomright)
                {
                    //Debug.Print("412");
                    setFall("412", CorrectDirection);
                }
                else if (!topleft && up && topright &&
                    bottomleft && down && !bottomright)
                {
                    //Debug.Print("413");
                    setFall("413", CorrectDirection);
                }
                else if (topleft && up && !topright &&
                    bottomleft && down && !bottomright)
                {
                    //Debug.Print("414");
                    setFall("414", CorrectDirection);
                }
                else if (!topleft && up && !topright &&
                    bottomleft && down && !bottomright)
                {
                    //Debug.Print("415");
                    setFall("415", CorrectDirection);
                }
                else if (!topleft && !up && !topright &&
                    bottomleft && down && !bottomright)
                {
                    //Debug.Print("416");
                    setFall("416", CorrectDirection);
                }
                else if (!topleft && up && topright &&
                    !bottomleft && down && !bottomright)
                {
                    //Debug.Print("417");
                    setFall("417", CorrectDirection);
                }
                else if (topleft && up && !topright &&
                    !bottomleft && down && !bottomright)
                {
                    //Debug.Print("418");
                    setFall("418", CorrectDirection);
                }
                else if (!topleft && up && !topright &&
                    !bottomleft && down && !bottomright)
                {
                    //Debug.Print("419");
                    setFall("419", CorrectDirection);
                }
                else if (!topleft && !up && !topright &&
                    !bottomleft && down && !bottomright)
                {
                    //Debug.Print("420");
                    setFall("420", CorrectDirection);
                }
                else if (!topleft && up && topright &&
                    !bottomleft && !down && !bottomright)
                {
                    //Debug.Print("421");
                    setFall("421", CorrectDirection);
                }
                else if (topleft && up && !topright &&
                    !bottomleft && !down && !bottomright)
                {
                    //Debug.Print("422");
                    setFall("422", CorrectDirection);
                }
                else if (!topleft && up && !topright &&
                    !bottomleft && !down && !bottomright)
                {
                    //Debug.Print("423");
                    setFall("423", CorrectDirection);
                }
                else if (!topleft && !up && !topright &&
                    !bottomleft && !down && !bottomright)
                {
                    //Debug.Print("424");
                    setFall("424", CorrectDirection);
                }
                else
                {
                    MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MyMessageBox.Show("No Solution!", "Wait? WTF?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setFall(string type, ushort direction)
        {
            ushort writeValue = 0;

            switch (type)
            {
                case "100":
                    writeValue = (ushort)TerrainUnitModel.Fall100;
                    break;
                case "101":
                    writeValue = (ushort)TerrainUnitModel.Fall101;
                    break;
                case "102":
                    writeValue = (ushort)TerrainUnitModel.Fall102;
                    break;
                case "103":
                    writeValue = (ushort)TerrainUnitModel.Fall103;
                    break;
                case "200":
                    writeValue = (ushort)TerrainUnitModel.Fall200;
                    break;
                case "201":
                    writeValue = (ushort)TerrainUnitModel.Fall201;
                    break;
                case "202":
                    writeValue = (ushort)TerrainUnitModel.Fall202;
                    break;
                case "203":
                    writeValue = (ushort)TerrainUnitModel.Fall203;
                    break;
                case "204":
                    writeValue = (ushort)TerrainUnitModel.Fall204;
                    break;
                case "205":
                    writeValue = (ushort)TerrainUnitModel.Fall205;
                    break;
                case "206":
                    writeValue = (ushort)TerrainUnitModel.Fall206;
                    break;
                case "207":
                    writeValue = (ushort)TerrainUnitModel.Fall207;
                    break;
                case "208":
                    writeValue = (ushort)TerrainUnitModel.Fall208;
                    break;
                case "300":
                    writeValue = (ushort)TerrainUnitModel.Fall300;
                    break;
                case "301":
                    writeValue = (ushort)TerrainUnitModel.Fall301;
                    break;
                case "302":
                    writeValue = (ushort)TerrainUnitModel.Fall302;
                    break;
                case "303":
                    writeValue = (ushort)TerrainUnitModel.Fall303;
                    break;
                case "304":
                    writeValue = (ushort)TerrainUnitModel.Fall304;
                    break;
                case "305":
                    writeValue = (ushort)TerrainUnitModel.Fall305;
                    break;
                case "306":
                    writeValue = (ushort)TerrainUnitModel.Fall306;
                    break;
                case "307":
                    writeValue = (ushort)TerrainUnitModel.Fall307;
                    break;
                case "308":
                    writeValue = (ushort)TerrainUnitModel.Fall308;
                    break;
                case "400":
                    writeValue = (ushort)TerrainUnitModel.Fall400;
                    break;
                case "401":
                    writeValue = (ushort)TerrainUnitModel.Fall401;
                    break;
                case "402":
                    writeValue = (ushort)TerrainUnitModel.Fall402;
                    break;
                case "403":
                    writeValue = (ushort)TerrainUnitModel.Fall403;
                    break;
                case "404":
                    writeValue = (ushort)TerrainUnitModel.Fall404;
                    break;
                case "405":
                    writeValue = (ushort)TerrainUnitModel.Fall405;
                    break;
                case "406":
                    writeValue = (ushort)TerrainUnitModel.Fall406;
                    break;
                case "407":
                    writeValue = (ushort)TerrainUnitModel.Fall407;
                    break;
                case "408":
                    writeValue = (ushort)TerrainUnitModel.Fall408;
                    break;
                case "409":
                    writeValue = (ushort)TerrainUnitModel.Fall409;
                    break;
                case "410":
                    writeValue = (ushort)TerrainUnitModel.Fall410;
                    break;
                case "411":
                    writeValue = (ushort)TerrainUnitModel.Fall411;
                    break;
                case "412":
                    writeValue = (ushort)TerrainUnitModel.Fall412;
                    break;
                case "413":
                    writeValue = (ushort)TerrainUnitModel.Fall413;
                    break;
                case "414":
                    writeValue = (ushort)TerrainUnitModel.Fall414;
                    break;
                case "415":
                    writeValue = (ushort)TerrainUnitModel.Fall415;
                    break;
                case "416":
                    writeValue = (ushort)TerrainUnitModel.Fall416;
                    break;
                case "417":
                    writeValue = (ushort)TerrainUnitModel.Fall417;
                    break;
                case "418":
                    writeValue = (ushort)TerrainUnitModel.Fall418;
                    break;
                case "419":
                    writeValue = (ushort)TerrainUnitModel.Fall419;
                    break;
                case "420":
                    writeValue = (ushort)TerrainUnitModel.Fall420;
                    break;
                case "421":
                    writeValue = (ushort)TerrainUnitModel.Fall421;
                    break;
                case "422":
                    writeValue = (ushort)TerrainUnitModel.Fall422;
                    break;
                case "423":
                    writeValue = (ushort)TerrainUnitModel.Fall423;
                    break;
                case "424":
                    writeValue = (ushort)TerrainUnitModel.Fall424;
                    break;
                default:
                    writeValue = (ushort)TerrainUnitModel.Base;
                    break;
            }

            setTerrainModel(writeValue);
            setTerrainAngle(direction);
            setTerrainVariation(0);
        }

        private bool[,] RotateMatrix(bool[,] input)
        {
            bool[,] NewMatrix = new bool[3, 3];

            bool[] serialize = new bool[9];
            int iterator = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    serialize[iterator] = input[i, j];
                    iterator++;
                }
            }

            iterator = 0;
            for (int i = 2; i >= 0; i--)
            {
                for (int j = 0; j < 3; j++)
                {
                    NewMatrix[j, i] = serialize[iterator];
                    iterator++;
                }
            }

            return NewMatrix;
        }
        public void changeRoadCorner()
        {
            ushort road = getRoadType();
            ushort CurrentDirection = getRoadAngle();

            if (isRoad0A())
            {
                setRoad(road, "0B", CurrentDirection);
            }
            else if (isRoad0B())
            {
                if (CurrentDirection == 0)
                    setRoad(road, "0B", 1);
                else if (CurrentDirection == 1)
                    setRoad(road, "0B", 2);
                else if (CurrentDirection == 2)
                    setRoad(road, "0B", 3);
                else
                    setRoad(road, "0A", 0);
            }
            else if (isRoad1A())
            {
                setRoad(road, "1B", CurrentDirection);
            }
            else if (isRoad1B())
            {
                setRoad(road, "1C", CurrentDirection);
            }
            else if (isRoad1C())
            {
                setRoad(road, "1A", CurrentDirection);
            }
            else if (isRoad2B())
            {
                setRoad(road, "2C", CurrentDirection);
            }
            else if (isRoad2C())
            {
                setRoad(road, "2B", CurrentDirection);
            }
            else if (isRoad3B())
            {
                setRoad(road, "3C", CurrentDirection);
            }
            else if (isRoad3C())
            {
                setRoad(road, "3B", CurrentDirection);
            }
        }

        public void changeCliffCorner()
        {
            ushort terrain = getTerrainModel();
            ushort CurrentDirection = getTerrainAngle();
            ushort CurrentElevation = getElevation();

            if (terrain == (ushort)TerrainUnitModel.Cliff2B)
            {
                setCliff("2C", CurrentElevation, CurrentDirection);
            }
            else if (terrain == (ushort)TerrainUnitModel.Cliff2C)
            {
                setCliff("2B", CurrentElevation, CurrentDirection);
            }
            else if (terrain == (ushort)TerrainUnitModel.Cliff3B)
            {
                setCliff("3C", CurrentElevation, CurrentDirection);
            }
            else if (terrain == (ushort)TerrainUnitModel.Cliff3C)
            {
                setCliff("3B", CurrentElevation, CurrentDirection);
            }
        }

        public void changeRiverCorner()
        {
            ushort terrain = getTerrainModel();
            ushort CurrentDirection = getTerrainAngle();
            ushort CurrentElevation = getElevation();

            if (terrain == (ushort)TerrainUnitModel.River2B)
            {
                setRiver("2C", CurrentElevation, CurrentDirection);
            }
            else if (terrain == (ushort)TerrainUnitModel.River2C)
            {
                setRiver("2B", CurrentElevation, CurrentDirection);
            }
            else if (terrain == (ushort)TerrainUnitModel.River3B)
            {
                setRiver("3C", CurrentElevation, CurrentDirection);
            }
            else if (terrain == (ushort)TerrainUnitModel.River3C)
            {
                setRiver("3B", CurrentElevation, CurrentDirection);
            }
        }
        public void setRoadModel(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, TerrainData, 6, 2);
        }
        public void setRoadAngle(ushort angle)
        {
            var bytes = BitConverter.GetBytes(angle);
            Buffer.BlockCopy(bytes, 0, TerrainData, 10, 2);
        }
        private void setTerrainModel(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, TerrainData, 0, 2);
        }
        public void setTerrainAngle(ushort angle)
        {
            var bytes = BitConverter.GetBytes(angle);
            Buffer.BlockCopy(bytes, 0, TerrainData, 4, 2);
        }
        public void setTerrainVariation(ushort num)
        {
            var bytes = BitConverter.GetBytes(num);
            Buffer.BlockCopy(bytes, 0, TerrainData, 2, 2);
        }
        public void setTerrainElevation(ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bytes, 0, TerrainData, 12, 2);
        }

        private Bitmap drawImage(int size, Color border, Color background, Color building, Color terrain, Color road, bool drawBorder, bool drawBackground, bool drawBuilding, bool drawTerrain, bool drawRoad)
        {
            Bitmap Bottom;
            Bottom = new Bitmap(size, size);

            using (Graphics gr = Graphics.FromImage(Bottom))
            {
                gr.SmoothingMode = SmoothingMode.None;

                if (drawBorder)
                    gr.Clear(border);

                if (drawBackground)
                {
                    SolidBrush backgroundBrush = new SolidBrush(background);
                    gr.FillRectangle(backgroundBrush, 1, 1, size - 2, size - 2);
                }

                if (drawTerrain)
                {
                    ushort CurrentTerrain = getTerrainModel();
                    ushort CurrentElevation = getElevation();

                    if (isCliff() || isRiver() || isFlat())
                    {
                        SolidBrush terrainBrush = new SolidBrush(terrain);

                        switch (CurrentTerrain)
                        {
                            case (ushort)TerrainUnitModel.Cliff0A:
                            case (ushort)TerrainUnitModel.Cliff0A_2:
                            case (ushort)TerrainUnitModel.Cliff0A_3:
                            case (ushort)TerrainUnitModel.River0A:
                                gr.FillRectangle(terrainBrush, 4, 4, size - 8, size - 8);
                                break;
                            case (ushort)TerrainUnitModel.Cliff1A:
                            case (ushort)TerrainUnitModel.Cliff1A_2:
                            case (ushort)TerrainUnitModel.Cliff1A_3:
                            case (ushort)TerrainUnitModel.River1A:
                                gr.FillRectangle(terrainBrush, 4, 4, size - 8, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Cliff2A:
                            case (ushort)TerrainUnitModel.River2A:
                                gr.FillRectangle(terrainBrush, 4, 1, size - 8, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.Cliff2B:
                            case (ushort)TerrainUnitModel.River2B:
                                Point[] Terrain2B = new Point[] { new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(terrainBrush, Terrain2B);
                                break;
                            case (ushort)TerrainUnitModel.Cliff2C:
                            case (ushort)TerrainUnitModel.River2C:
                                Point[] Terrain2C = new Point[] { new Point(4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(terrainBrush, Terrain2C);
                                break;
                            case (ushort)TerrainUnitModel.Cliff3A:
                            case (ushort)TerrainUnitModel.River3A:
                                Point[] Terrain3A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(terrainBrush, Terrain3A);
                                break;
                            case (ushort)TerrainUnitModel.Cliff3B:
                            case (ushort)TerrainUnitModel.River3B:
                                Point[] Terrain3B = new Point[] { new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(terrainBrush, Terrain3B);
                                break;
                            case (ushort)TerrainUnitModel.Cliff3C:
                            case (ushort)TerrainUnitModel.River3C:
                                gr.FillRectangle(terrainBrush, 4, 4, size - 5, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Cliff4A:
                            case (ushort)TerrainUnitModel.River4A:
                                Point[] Terrain4A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(terrainBrush, Terrain4A);
                                break;
                            case (ushort)TerrainUnitModel.Cliff4B:
                            case (ushort)TerrainUnitModel.River4B:
                                Point[] Terrain4B = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(terrainBrush, Terrain4B);
                                break;
                            case (ushort)TerrainUnitModel.Cliff4C:
                            case (ushort)TerrainUnitModel.River4C:
                                Point[] Terrain4C = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(terrainBrush, Terrain4C);
                                break;
                            case (ushort)TerrainUnitModel.Cliff5A:
                            case (ushort)TerrainUnitModel.River5A:
                                Point[] Terrain5A = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(terrainBrush, Terrain5A);
                                break;
                            case (ushort)TerrainUnitModel.Cliff5B:
                            case (ushort)TerrainUnitModel.River5B:
                                gr.FillRectangle(terrainBrush, 4, 1, size - 5, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.Cliff6A:
                            case (ushort)TerrainUnitModel.River6A:
                                Point[] Terrain6A = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(terrainBrush, Terrain6A);
                                break;
                            case (ushort)TerrainUnitModel.Cliff6B:
                            case (ushort)TerrainUnitModel.River6B:
                                Point[] Terrain6B = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(terrainBrush, Terrain6B);
                                break;
                            case (ushort)TerrainUnitModel.Cliff7A:
                            case (ushort)TerrainUnitModel.River7A:
                                Point[] Terrain7A = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(terrainBrush, Terrain7A);
                                break;
                            case (ushort)TerrainUnitModel.Cliff8:
                                if (CurrentElevation != 0)
                                    gr.FillRectangle(terrainBrush, 1, 1, size - 2, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.River8A:
                                gr.FillRectangle(terrainBrush, 1, 1, size - 2, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.Base:
                                if (CurrentElevation != 0)
                                    gr.FillRectangle(terrainBrush, 1, 1, size - 2, size - 2);
                                break;
                            default:
                                gr.FillRectangle(terrainBrush, 4, 4, size - 8, size - 8);
                                break;
                        }
                    }
                    else
                    {
                        SolidBrush terrainBrush = new SolidBrush(terrain);
                        LinearGradientBrush linGrBrush = new LinearGradientBrush(
                           new Point(10, 0),
                           new Point(10, size),
                           TerrainColor[(int)TerrainType.Fall],
                           TerrainColor[(int)TerrainType.River]);

                        switch (CurrentTerrain)
                        {
                            case (ushort)TerrainUnitModel.Fall100:
                                gr.FillRectangle(linGrBrush, 4, 1, size - 8, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.Fall101:
                                gr.FillRectangle(linGrBrush, 4, 1, size - 8, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall102:
                                gr.FillRectangle(linGrBrush, 4, 4, size - 8, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall103:
                                gr.FillRectangle(linGrBrush, 4, 4, size - 8, size - 8);
                                break;
                            case (ushort)TerrainUnitModel.Fall200:
                                gr.FillRectangle(linGrBrush, 1, 1, size - 5, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.Fall201:
                                Point[] Terrain201 = new Point[] { new Point(1, 1), new Point(size - 4, 1), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain201);
                                break;
                            case (ushort)TerrainUnitModel.Fall202:
                                gr.FillRectangle(linGrBrush, 1, 1, size - 5, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall203:
                                Point[] Terrain203 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain203);
                                break;
                            case (ushort)TerrainUnitModel.Fall204:
                                gr.FillRectangle(linGrBrush, 1, 4, size - 5, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall205:
                                Point[] Terrain205 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain205);
                                break;
                            case (ushort)TerrainUnitModel.Fall206:
                                Point[] Terrain206 = new Point[] { new Point(1, 4), new Point(size - 4, 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain206);
                                break;
                            case (ushort)TerrainUnitModel.Fall207:
                                Point[] Terrain207 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain207);
                                break;
                            case (ushort)TerrainUnitModel.Fall208:
                                gr.FillRectangle(linGrBrush, 1, 4, size - 5, size - 8);
                                break;
                            case (ushort)TerrainUnitModel.Fall300:
                                gr.FillRectangle(linGrBrush, 4, 1, size - 5, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.Fall301:
                                Point[] Terrain301 = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain301);
                                break;
                            case (ushort)TerrainUnitModel.Fall302:
                                gr.FillRectangle(linGrBrush, 4, 1, size - 5, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall303:
                                Point[] Terrain303 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain303);
                                break;
                            case (ushort)TerrainUnitModel.Fall304:
                                gr.FillRectangle(linGrBrush, 4, 4, size - 5, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall305:
                                Point[] Terrain305 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain305);
                                break;
                            case (ushort)TerrainUnitModel.Fall306:
                                Point[] Terrain306 = new Point[] { new Point(4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain306);
                                break;
                            case (ushort)TerrainUnitModel.Fall307:
                                Point[] Terrain307 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(4, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain307);
                                break;
                            case (ushort)TerrainUnitModel.Fall308:
                                gr.FillRectangle(linGrBrush, 4, 4, size - 5, size - 8);
                                break;
                            case (ushort)TerrainUnitModel.Fall400:
                                gr.FillRectangle(linGrBrush, 1, 1, size - 2, size - 2);
                                break;
                            case (ushort)TerrainUnitModel.Fall401:
                                Point[] Terrain401 = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain401);
                                break;
                            case (ushort)TerrainUnitModel.Fall402:
                                Point[] Terrain402 = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain402);
                                break;
                            case (ushort)TerrainUnitModel.Fall403:
                                gr.FillRectangle(linGrBrush, 1, 1, size - 2, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall404:
                                Point[] Terrain404 = new Point[] { new Point(1, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain404);
                                break;
                            case (ushort)TerrainUnitModel.Fall405:
                                Point[] Terrain405 = new Point[] { new Point(1, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(1, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain405);
                                break;
                            case (ushort)TerrainUnitModel.Fall406:
                                Point[] Terrain406 = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain406);
                                break;
                            case (ushort)TerrainUnitModel.Fall407:
                                Point[] Terrain407 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain407);
                                break;
                            case (ushort)TerrainUnitModel.Fall408:
                                gr.FillRectangle(linGrBrush, 1, 4, size - 2, size - 5);
                                break;
                            case (ushort)TerrainUnitModel.Fall409:
                                Point[] Terrain409 = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain409);
                                break;
                            case (ushort)TerrainUnitModel.Fall410:
                                Point[] Terrain410 = new Point[] { new Point(1, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain410);
                                break;
                            case (ushort)TerrainUnitModel.Fall411:
                                Point[] Terrain411 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain411);
                                break;
                            case (ushort)TerrainUnitModel.Fall412:
                                Point[] Terrain412 = new Point[] { new Point(1, 4), new Point(size - 1, 4), new Point(size - 1, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain412);
                                break;
                            case (ushort)TerrainUnitModel.Fall413:
                                Point[] Terrain413 = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain413);
                                break;
                            case (ushort)TerrainUnitModel.Fall414:
                                Point[] Terrain414 = new Point[] { new Point(1, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain414);
                                break;
                            case (ushort)TerrainUnitModel.Fall415:
                                Point[] Terrain415 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain415);
                                break;
                            case (ushort)TerrainUnitModel.Fall416:
                                Point[] Terrain416 = new Point[] { new Point(1, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(1, size - 1) };
                                gr.FillPolygon(linGrBrush, Terrain416);
                                break;
                            case (ushort)TerrainUnitModel.Fall417:
                                Point[] Terrain417 = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain417);
                                break;
                            case (ushort)TerrainUnitModel.Fall418:
                                Point[] Terrain418 = new Point[] { new Point(1, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain418);
                                break;
                            case (ushort)TerrainUnitModel.Fall419:
                                Point[] Terrain419 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain419);
                                break;
                            case (ushort)TerrainUnitModel.Fall420:
                                Point[] Terrain420 = new Point[] { new Point(1, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(size - 4, size - 4), new Point(size - 4, size - 1), new Point(4, size - 1), new Point(4, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain420);
                                break;
                            case (ushort)TerrainUnitModel.Fall421:
                                Point[] Terrain421 = new Point[] { new Point(4, 1), new Point(size - 1, 1), new Point(size - 1, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain421);
                                break;
                            case (ushort)TerrainUnitModel.Fall422:
                                Point[] Terrain422 = new Point[] { new Point(1, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(1, size - 4) };
                                gr.FillPolygon(linGrBrush, Terrain422);
                                break;
                            case (ushort)TerrainUnitModel.Fall423:
                                Point[] Terrain423 = new Point[] { new Point(4, 1), new Point(size - 4, 1), new Point(size - 4, 4), new Point(size - 1, 4), new Point(size - 1, size - 4), new Point(1, size - 4), new Point(1, 4), new Point(4, 4) };
                                gr.FillPolygon(linGrBrush, Terrain423);
                                break;
                            case (ushort)TerrainUnitModel.Fall424:
                                gr.FillRectangle(linGrBrush, 1, 4, size - 2, size - 8);
                                break;
                            default:
                                gr.FillRectangle(terrainBrush, 4, 4, size - 8, size - 8);
                                break;
                        }
                    }
                }

                if (drawBuilding)
                {
                    SolidBrush buildingBrush = new SolidBrush(building);
                    gr.FillRectangle(buildingBrush, 5, 5, size - 10, size - 10);
                }
            }

            if (getTerrainAngle() == 1)
                Bottom.RotateFlip(RotateFlipType.Rotate270FlipNone);
            else if (getTerrainAngle() == 2)
                Bottom.RotateFlip(RotateFlipType.Rotate180FlipNone);
            else if (getTerrainAngle() == 3)
                Bottom.RotateFlip(RotateFlipType.Rotate90FlipNone);



            Bitmap Top;
            Top = new Bitmap(size, size);

            using (Graphics gr = Graphics.FromImage(Top))
            {
                if (drawRoad)
                {
                    SolidBrush roadBrush = new SolidBrush(road);
                    if (isRoad0A())
                    {
                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 16);
                    }
                    else if (isRoad0B())
                    {
                        Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
                        gr.FillPie(roadBrush, pieRect, -90, -90);
                    }
                    else if (isRoad1A())
                    {
                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 9);
                    }
                    else if (isRoad1B())
                    {
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                        Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
                        gr.FillPie(roadBrush, pieRect, -90, -90);

                    }
                    else if (isRoad1C())
                    {
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                        Rectangle pieRect = new Rectangle(8 - (size - 16), 8, (size - 16) * 2, (size - 16) * 2);
                        gr.FillPie(roadBrush, pieRect, 0, -90);
                    }
                    else if (isRoad2A())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad2B())
                    {
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                        Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
                        gr.FillPie(roadBrush, pieRect, -90, -90);

                    }
                    else if (isRoad2C())
                    {
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 16);
                    }
                    else if (isRoad3A())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad3B())
                    {
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                        Rectangle pieRect = new Rectangle(8, 8, (size - 16) * 2, (size - 16) * 2);
                        gr.FillPie(roadBrush, pieRect, -90, -90);
                    }
                    else if (isRoad3C())
                    {
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 16);
                    }
                    else if (isRoad4A())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad4B())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 1, 7, 7); //Top Right

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad4C())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 1, 8, 7, size - 16); //Left

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad5A())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 1, 8, 7, size - 16); //Left
                        gr.FillRectangle(roadBrush, 1, 8 + (size - 16), 7, 7); //Bottom Left

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad5B())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom

                        gr.FillRectangle(roadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad6A())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 1, 8, 7, size - 16); //Left
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                        gr.FillRectangle(roadBrush, 1, 8 + (size - 16), 7, 7); //Bottom Left

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad6B())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 1, 8, 7, size - 16); //Left
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                        gr.FillRectangle(roadBrush, 1, 1, 7, 7); //Top Left

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad7A())
                    {
                        gr.FillRectangle(roadBrush, 8, 1, size - 16, 7); //Top
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8, 7, size - 16); //Right
                        gr.FillRectangle(roadBrush, 8, 8 + (size - 16), size - 16, 7); //Bottom
                        gr.FillRectangle(roadBrush, 1, 8, 7, size - 16); //Left
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 1, 7, 7); //Top Right
                        gr.FillRectangle(roadBrush, 1, 1, 7, 7); //Top Left
                        gr.FillRectangle(roadBrush, 8 + (size - 16), 8 + (size - 16), 7, 7); //Bottom Right

                        gr.FillRectangle(roadBrush, 8, 8, size - 16, size - 10);
                    }
                    else if (isRoad8A())
                    {
                        gr.FillRectangle(roadBrush, 1, 1, size - 2, size - 2);
                    }
                    else
                    {
                        gr.FillRectangle(roadBrush, 5, 5, size - 10, size - 10);
                    }
                }
            }

            if (getRoadAngle() == 1)
                Top.RotateFlip(RotateFlipType.Rotate270FlipNone);
            else if (getRoadAngle() == 2)
                Top.RotateFlip(RotateFlipType.Rotate180FlipNone);
            else if (getRoadAngle() == 3)
                Top.RotateFlip(RotateFlipType.Rotate90FlipNone);

            Bitmap Final = Bottom;

            using (Graphics graphics = Graphics.FromImage(Final))
            {
                var cm = new ColorMatrix();
                cm.Matrix33 = 1;

                var ia = new ImageAttributes();
                ia.SetColorMatrix(cm);

                graphics.DrawImage(Top, new Rectangle(0, 0, Top.Width, Top.Height), 0, 0, Top.Width, Top.Height, GraphicsUnit.Pixel, ia);
            }

            return Final;
        }

        public enum TerrainType : int
        {
            RoadWood = 0,       //Wooden path
            RoadTile = 1,       //Terra-cotta tiles
            RoadSand = 2,       //Sand path
            RoadPattern = 3,    //Arched tile path
            RoadDarkSoil = 4,   //Dark dirt path
            RoadBrick = 5,      //Brick path
            RoadStone = 6,      //Stone path
            RoadSoil = 7,       //Dirt path
            Fall = 10,
            Cliff = 11,
            River = 12,
            Elevation0 = 19,
            Elevation1 = 20,
            Elevation2 = 21,
            Elevation3 = 22,
            Elevation4 = 23,
            Elevation5 = 24,
            Elevation6 = 25,
            Elevation7 = 26,
            Elevation8 = 27,
        }
        public static readonly Dictionary<int, Color> TerrainColor = new Dictionary<int, Color>
        {
            {0, Color.FromArgb(170, 105, 89)},
            {1, Color.FromArgb(232, 178, 128)},
            {2, Color.FromArgb(234, 206, 142)},
            {3, Color.FromArgb(133, 155, 149)},
            {4, Color.FromArgb(185, 128, 95)},
            {5, Color.FromArgb(239, 159, 100)},
            {6, Color.FromArgb(125, 142, 138)},
            {7, Color.FromArgb(218, 179, 116)},
            {10, Color.FromArgb(52, 36, 237)},
            {11, Color.FromArgb(17, 43, 18)},
            {12, Color.FromArgb(52, 170, 247)},
            {19, Color.FromArgb(70, 116, 71)},
            {20, Color.FromArgb(55, 92, 56)},
            {21, Color.FromArgb(44, 74, 45)},
            {22, Color.FromArgb(33, 56, 34)},
            {23, Color.FromArgb(255, 209, 253)},
            {24, Color.FromArgb(252, 174, 249)},
            {25, Color.FromArgb(252, 136, 248)},
            {26, Color.FromArgb(250, 90, 244)},
            {27, Color.FromArgb(250, 47, 242)},
        };
        public enum TerrainUnitModel : ushort
        {
            Base = 0x00,
            River0A = 0x01,
            River1A = 0x04,
            River2A = 0x07,
            River2B = 0x08,
            River2C = 0x09,
            River3A = 0x0A,
            River3B = 0x0B,
            River3C = 0x0C,
            River4A = 0x0D,
            River4B = 0x0E,
            River4C = 0x0F,
            River5A = 0x10,
            River5B = 0x11,
            River6A = 0x12,
            River6B = 0x13,
            River7A = 0x14,
            River8A = 0x15,
            Cliff0A = 0x16,
            Cliff0A_2 = 0x17,
            Cliff0A_3 = 0x18,
            Cliff1A = 0x19,
            Cliff1A_2 = 0x1A,
            Cliff1A_3 = 0x1B,
            Cliff2A = 0x1C,
            Cliff2C = 0x1D,
            Cliff3A = 0x1E,
            Cliff3B = 0x1F,
            Cliff3C = 0x20,
            Cliff4A = 0x21,
            Cliff4B = 0x22,
            Cliff4C = 0x23,
            Cliff5A = 0x24,
            Cliff5B = 0x25,
            Cliff6A = 0x26,
            Cliff6B = 0x27,
            Cliff7A = 0x28,
            Cliff8 = 0x29,
            Fall101 = 0x3A,
            Fall100 = 0x3B,
            Fall300 = 0x3C,
            Fall301 = 0x3D,
            Fall302 = 0x3E,
            Fall200 = 0x3F,
            Fall201 = 0x40,
            Fall202 = 0x41,
            Fall400 = 0x42,
            Fall401 = 0x43,
            Fall402 = 0x44,
            Fall403 = 0x45,
            Fall404 = 0x46,
            RoadSoil0A = 0x47,
            RoadSoil1A = 0x48,
            RoadSoil0B = 0x49,
            RoadSoil1B = 0x4B,
            RoadSoil1C = 0x4C,
            RoadSoil2A = 0x4D,
            RoadSoil2B = 0x4E,
            RoadSoil2C = 0x4F,
            RoadSoil3A = 0x50,
            RoadSoil3B = 0x51,
            RoadSoil3C = 0x52,
            RoadSoil4A = 0x53,
            RoadSoil4B = 0x54,
            RoadSoil4C = 0x55,
            RoadSoil5A = 0x56,
            RoadSoil5B = 0x57,
            RoadSoil6A = 0x58,
            RoadSoil6B = 0x59,
            RoadSoil7A = 0x5A,
            RoadSoil8A = 0x5B,
            RoadStone0A = 0x5C,
            RoadStone0B = 0x5D,
            RoadStone1A = 0x5F,
            RoadStone1B = 0x60,
            RoadStone1C = 0x61,
            RoadStone2A = 0x62,
            RoadStone2B = 0x63,
            RoadStone2C = 0x64,
            RoadStone3A = 0x65,
            RoadStone3B = 0x66,
            RoadStone3C = 0x67,
            RoadStone4A = 0x68,
            RoadStone4B = 0x69,
            RoadStone4C = 0x6A,
            RoadStone5A = 0x6B,
            RoadStone5B = 0x6C,
            RoadStone6A = 0x6D,
            RoadStone6B = 0x6E,
            RoadStone7A = 0x6F,
            RoadStone8A = 0x70,
            Fall103 = 0x71,
            Fall102 = 0x72,
            Fall303 = 0x73,
            Fall304 = 0x74,
            Fall305 = 0x75,
            Fall306 = 0x76,
            Fall307 = 0x77,
            Fall308 = 0x78,
            Fall203 = 0x79,
            Fall204 = 0x7A,
            Fall205 = 0x7B,
            Fall206 = 0x7C,
            Fall207 = 0x7D,
            Fall208 = 0x7E,
            Fall405 = 0x7F,
            Fall406 = 0x80,
            Fall407 = 0x81,
            Fall408 = 0x82,
            Fall410 = 0x83,
            Fall409 = 0x84,
            Fall411 = 0x85,
            Fall412 = 0x86,
            Fall414 = 0x87,
            Fall413 = 0x88,
            Fall415 = 0x89,
            Fall416 = 0x8A,
            Fall418 = 0x8B,
            Fall417 = 0x8C,
            Fall419 = 0x8D,
            Fall420 = 0x8E,
            Fall422 = 0x8F,
            Fall421 = 0x90,
            Fall423 = 0x91,
            Fall424 = 0x92,
            Cliff2B = 0x93,
            RoadBrick0A = 0x94,
            RoadBrick0B = 0x95,
            RoadBrick1A = 0x97,
            RoadBrick1B = 0x98,
            RoadBrick1C = 0x99,
            RoadBrick2A = 0x9A,
            RoadBrick2B = 0x9B,
            RoadBrick2C = 0x9C,
            RoadBrick3A = 0x9D,
            RoadBrick3B = 0x9E,
            RoadBrick3C = 0x9F,
            RoadBrick4A = 0xA0,
            RoadBrick4B = 0xA1,
            RoadBrick4C = 0xA2,
            RoadBrick5A = 0xA3,
            RoadBrick5B = 0xA4,
            RoadBrick6A = 0xA5,
            RoadBrick6B = 0xA6,
            RoadBrick7A = 0xA7,
            RoadBrick8A = 0xA8,
            RoadDarkSoil0A = 0xA9,
            RoadDarkSoil0B = 0xAA,
            RoadDarkSoil1A = 0xAC,
            RoadDarkSoil1B = 0xAD,
            RoadDarkSoil1C = 0xAE,
            RoadDarkSoil2A = 0xAF,
            RoadDarkSoil2B = 0xB0,
            RoadDarkSoil2C = 0xB1,
            RoadDarkSoil3A = 0xB2,
            RoadDarkSoil3B = 0xB3,
            RoadDarkSoil3C = 0xB4,
            RoadDarkSoil4A = 0xB5,
            RoadDarkSoil4B = 0xB6,
            RoadDarkSoil4C = 0xB7,
            RoadDarkSoil5A = 0xB8,
            RoadDarkSoil5B = 0xB9,
            RoadDarkSoil6A = 0xBA,
            RoadDarkSoil6B = 0xBB,
            RoadDarkSoil7A = 0xBC,
            RoadDarkSoil8A = 0xBD,
            RoadFanPattern0A = 0xBE,
            RoadFanPattern0B = 0xBF,
            RoadFanPattern1A = 0xC1,
            RoadFanPattern1B = 0xC2,
            RoadFanPattern1C = 0xC3,
            RoadFanPattern2A = 0xC4,
            RoadFanPattern2B = 0xC5,
            RoadFanPattern2C = 0xC6,
            RoadFanPattern3A = 0xC7,
            RoadFanPattern3B = 0xC8,
            RoadFanPattern3C = 0xC9,
            RoadFanPattern4A = 0xCA,
            RoadFanPattern4B = 0xCB,
            RoadFanPattern4C = 0xCC,
            RoadFanPattern5A = 0xCD,
            RoadFanPattern5B = 0xCE,
            RoadFanPattern6A = 0xCF,
            RoadFanPattern6B = 0xD0,
            RoadFanPattern7A = 0xD1,
            RoadFanPattern8A = 0xD2,
            RoadSand0A = 0xD3,
            RoadSand0B = 0xD4,
            RoadSand1A = 0xD6,
            RoadSand1B = 0xD7,
            RoadSand1C = 0xD8,
            RoadSand2A = 0xD9,
            RoadSand2B = 0xDA,
            RoadSand2C = 0xDB,
            RoadSand3A = 0xDC,
            RoadSand3B = 0xDD,
            RoadSand3C = 0xDE,
            RoadSand4A = 0xDF,
            RoadSand4B = 0xE0,
            RoadSand4C = 0xE1,
            RoadSand5A = 0xE2,
            RoadSand5B = 0xE3,
            RoadSand6A = 0xE4,
            RoadSand6B = 0xE5,
            RoadSand7A = 0xE6,
            RoadSand8A = 0xE7,
            RoadTile0A = 0xE8,
            RoadTile0B = 0xE9,
            RoadTile1A = 0xEB,
            RoadTile1B = 0xEC,
            RoadTile1C = 0xED,
            RoadTile2A = 0xEE,
            RoadTile2B = 0xEF,
            RoadTile2C = 0xF0,
            RoadTile3A = 0xF1,
            RoadTile3B = 0xF2,
            RoadTile3C = 0xF3,
            RoadTile4A = 0xF4,
            RoadTile4B = 0xF5,
            RoadTile4C = 0xF6,
            RoadTile5A = 0xF7,
            RoadTile5B = 0xF8,
            RoadTile6A = 0xF9,
            RoadTile6B = 0xFA,
            RoadTile7A = 0xFB,
            RoadTile8A = 0xFC,
            RoadWood0A = 0xFD,
            RoadWood0B = 0xFE,
            RoadWood1A = 0x100,
            RoadWood1B = 0x101,
            RoadWood1C = 0x102,
            RoadWood2A = 0x103,
            RoadWood2B = 0x104,
            RoadWood2C = 0x105,
            RoadWood3A = 0x106,
            RoadWood3B = 0x107,
            RoadWood3C = 0x108,
            RoadWood4A = 0x109,
            RoadWood4B = 0x10A,
            RoadWood4C = 0x10B,
            RoadWood5A = 0x10C,
            RoadWood5B = 0x10D,
            RoadWood6A = 0x10E,
            RoadWood6B = 0x10F,
            RoadWood7A = 0x110,
            RoadWood8A = 0x111,
        }
        public static Dictionary<ushort, string> TerrainName = new Dictionary<ushort, string>
        {
            {0x00,"Base"},
            {0x01,"River0A"},
            {0x04,"River1A"},
            {0x07,"River2A"},
            {0x08,"River2B"},
            {0x09,"River2C"},
            {0x0A,"River3A"},
            {0x0B,"River3B"},
            {0x0C,"River3C"},
            {0x0D,"River4A"},
            {0x0E,"River4B"},
            {0x0F,"River4C"},
            {0x10,"River5A"},
            {0x11,"River5B"},
            {0x12,"River6A"},
            {0x13,"River6B"},
            {0x14,"River7A"},
            {0x15,"River8A"},
            {0x16,"Cliff0A"},
            {0x17,"Cliff0A_2"},
            {0x18,"Cliff0A_3"},
            {0x19,"Cliff1A"},
            {0x1A,"Cliff1A_2"},
            {0x1B,"Cliff1A_3"},
            {0x1C,"Cliff2A"},
            {0x1D,"Cliff2C"},
            {0x1E,"Cliff3A"},
            {0x1F,"Cliff3B"},
            {0x20,"Cliff3C"},
            {0x21,"Cliff4A"},
            {0x22,"Cliff4B"},
            {0x23,"Cliff4C"},
            {0x24,"Cliff5A"},
            {0x25,"Cliff5B"},
            {0x26,"Cliff6A"},
            {0x27,"Cliff6B"},
            {0x28,"Cliff7A"},
            {0x29,"Cliff8"},
            {0x3A,"Fall101"},
            {0x3B,"Fall100"},
            {0x3C,"Fall300"},
            {0x3D,"Fall301"},
            {0x3E,"Fall302"},
            {0x3F,"Fall200"},
            {0x40,"Fall201"},
            {0x41,"Fall202"},
            {0x42,"Fall400"},
            {0x43,"Fall401"},
            {0x44,"Fall402"},
            {0x45,"Fall403"},
            {0x46,"Fall404"},
            {0x47,"RoadSoil0A"},
            {0x48,"RoadSoil1A"},
            {0x49,"RoadSoil0B"},
            {0x4B,"RoadSoil1B"},
            {0x4C,"RoadSoil1C"},
            {0x4D,"RoadSoil2A"},
            {0x4E,"RoadSoil2B"},
            {0x4F,"RoadSoil2C"},
            {0x50,"RoadSoil3A"},
            {0x51,"RoadSoil3B"},
            {0x52,"RoadSoil3C"},
            {0x53,"RoadSoil4A"},
            {0x54,"RoadSoil4B"},
            {0x55,"RoadSoil4C"},
            {0x56,"RoadSoil5A"},
            {0x57,"RoadSoil5B"},
            {0x58,"RoadSoil6A"},
            {0x59,"RoadSoil6B"},
            {0x5A,"RoadSoil7A"},
            {0x5B,"RoadSoil8A"},
            {0x5C,"RoadStone0A"},
            {0x5D,"RoadStone0B"},
            {0x5F,"RoadStone1A"},
            {0x60,"RoadStone1B"},
            {0x61,"RoadStone1C"},
            {0x62,"RoadStone2A"},
            {0x63,"RoadStone2B"},
            {0x64,"RoadStone2C"},
            {0x65,"RoadStone3A"},
            {0x66,"RoadStone3B"},
            {0x67,"RoadStone3C"},
            {0x68,"RoadStone4A"},
            {0x69,"RoadStone4B"},
            {0x6A,"RoadStone4C"},
            {0x6B,"RoadStone5A"},
            {0x6C,"RoadStone5B"},
            {0x6D,"RoadStone6A"},
            {0x6E,"RoadStone6B"},
            {0x6F,"RoadStone7A"},
            {0x70,"RoadStone8A"},
            {0x71,"Fall103"},
            {0x72,"Fall102"},
            {0x73,"Fall303"},
            {0x74,"Fall304"},
            {0x75,"Fall305"},
            {0x76,"Fall306"},
            {0x77,"Fall307"},
            {0x78,"Fall308"},
            {0x79,"Fall203"},
            {0x7A,"Fall204"},
            {0x7B,"Fall205"},
            {0x7C,"Fall206"},
            {0x7D,"Fall207"},
            {0x7E,"Fall208"},
            {0x7F,"Fall405"},
            {0x80,"Fall406"},
            {0x81,"Fall407"},
            {0x82,"Fall408"},
            {0x83,"Fall410"},
            {0x84,"Fall409"},
            {0x85,"Fall411"},
            {0x86,"Fall412"},
            {0x87,"Fall414"},
            {0x88,"Fall413"},
            {0x89,"Fall415"},
            {0x8A,"Fall416"},
            {0x8B,"Fall418"},
            {0x8C,"Fall417"},
            {0x8D,"Fall419"},
            {0x8E,"Fall420"},
            {0x8F,"Fall422"},
            {0x90,"Fall421"},
            {0x91,"Fall423"},
            {0x92,"Fall424"},
            {0x93,"Cliff2B"},
            {0x94,"RoadBrick0A"},
            {0x95,"RoadBrick0B"},
            {0x97,"RoadBrick1A"},
            {0x98,"RoadBrick1B"},
            {0x99,"RoadBrick1C"},
            {0x9A,"RoadBrick2A"},
            {0x9B,"RoadBrick2B"},
            {0x9C,"RoadBrick2C"},
            {0x9D,"RoadBrick3A"},
            {0x9E,"RoadBrick3B"},
            {0x9F,"RoadBrick3C"},
            {0xA0,"RoadBrick4A"},
            {0xA1,"RoadBrick4B"},
            {0xA2,"RoadBrick4C"},
            {0xA3,"RoadBrick5A"},
            {0xA4,"RoadBrick5B"},
            {0xA5,"RoadBrick6A"},
            {0xA6,"RoadBrick6B"},
            {0xA7,"RoadBrick7A"},
            {0xA8,"RoadBrick8A"},
            {0xA9,"RoadDarkSoil0A"},
            {0xAA,"RoadDarkSoil0B"},
            {0xAC,"RoadDarkSoil1A"},
            {0xAD,"RoadDarkSoil1B"},
            {0xAE,"RoadDarkSoil1C"},
            {0xAF,"RoadDarkSoil2A"},
            {0xB0,"RoadDarkSoil2B"},
            {0xB1,"RoadDarkSoil2C"},
            {0xB2,"RoadDarkSoil3A"},
            {0xB3,"RoadDarkSoil3B"},
            {0xB4,"RoadDarkSoil3C"},
            {0xB5,"RoadDarkSoil4A"},
            {0xB6,"RoadDarkSoil4B"},
            {0xB7,"RoadDarkSoil4C"},
            {0xB8,"RoadDarkSoil5A"},
            {0xB9,"RoadDarkSoil5B"},
            {0xBA,"RoadDarkSoil6A"},
            {0xBB,"RoadDarkSoil6B"},
            {0xBC,"RoadDarkSoil7A"},
            {0xBD,"RoadDarkSoil8A"},
            {0xBE,"RoadFanPattern0A"},
            {0xBF,"RoadFanPattern0B"},
            {0xC1,"RoadFanPattern1A"},
            {0xC2,"RoadFanPattern1B"},
            {0xC3,"RoadFanPattern1C"},
            {0xC4,"RoadFanPattern2A"},
            {0xC5,"RoadFanPattern2B"},
            {0xC6,"RoadFanPattern2C"},
            {0xC7,"RoadFanPattern3A"},
            {0xC8,"RoadFanPattern3B"},
            {0xC9,"RoadFanPattern3C"},
            {0xCA,"RoadFanPattern4A"},
            {0xCB,"RoadFanPattern4B"},
            {0xCC,"RoadFanPattern4C"},
            {0xCD,"RoadFanPattern5A"},
            {0xCE,"RoadFanPattern5B"},
            {0xCF,"RoadFanPattern6A"},
            {0xD0,"RoadFanPattern6B"},
            {0xD1,"RoadFanPattern7A"},
            {0xD2,"RoadFanPattern8A"},
            {0xD3,"RoadSand0A"},
            {0xD4,"RoadSand0B"},
            {0xD6,"RoadSand1A"},
            {0xD7,"RoadSand1B"},
            {0xD8,"RoadSand1C"},
            {0xD9,"RoadSand2A"},
            {0xDA,"RoadSand2B"},
            {0xDB,"RoadSand2C"},
            {0xDC,"RoadSand3A"},
            {0xDD,"RoadSand3B"},
            {0xDE,"RoadSand3C"},
            {0xDF,"RoadSand4A"},
            {0xE0,"RoadSand4B"},
            {0xE1,"RoadSand4C"},
            {0xE2,"RoadSand5A"},
            {0xE3,"RoadSand5B"},
            {0xE4,"RoadSand6A"},
            {0xE5,"RoadSand6B"},
            {0xE6,"RoadSand7A"},
            {0xE7,"RoadSand8A"},
            {0xE8,"RoadTile0A"},
            {0xE9,"RoadTile0B"},
            {0xEB,"RoadTile1A"},
            {0xEC,"RoadTile1B"},
            {0xED,"RoadTile1C"},
            {0xEE,"RoadTile2A"},
            {0xEF,"RoadTile2B"},
            {0xF0,"RoadTile2C"},
            {0xF1,"RoadTile3A"},
            {0xF2,"RoadTile3B"},
            {0xF3,"RoadTile3C"},
            {0xF4,"RoadTile4A"},
            {0xF5,"RoadTile4B"},
            {0xF6,"RoadTile4C"},
            {0xF7,"RoadTile5A"},
            {0xF8,"RoadTile5B"},
            {0xF9,"RoadTile6A"},
            {0xFA,"RoadTile6B"},
            {0xFB,"RoadTile7A"},
            {0xFC,"RoadTile8A"},
            {0xFD,"RoadWood0A"},
            {0xFE,"RoadWood0B"},
            {0x100,"RoadWood1A"},
            {0x101,"RoadWood1B"},
            {0x102,"RoadWood1C"},
            {0x103,"RoadWood2A"},
            {0x104,"RoadWood2B"},
            {0x105,"RoadWood2C"},
            {0x106,"RoadWood3A"},
            {0x107,"RoadWood3B"},
            {0x108,"RoadWood3C"},
            {0x109,"RoadWood4A"},
            {0x10A,"RoadWood4B"},
            {0x10B,"RoadWood4C"},
            {0x10C,"RoadWood5A"},
            {0x10D,"RoadWood5B"},
            {0x10E,"RoadWood6A"},
            {0x10F,"RoadWood6B"},
            {0x110,"RoadWood7A"},
            {0x111,"RoadWood8A"},
        };
    }
}
