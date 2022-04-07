using System;
using System.Collections.Generic;

namespace ACNHPokerCore
{
    public class TerrainUnit
    {
        private byte[] TerrainData;

        public TerrainUnit(byte[] terrainData)
        {
            TerrainData = terrainData;
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

        public bool HasRoad()
        {
            ushort road = getRoadModel();
            if (road >= (ushort)TerrainUnitModel.RoadBrick0A || (road >= (ushort)TerrainUnitModel.RoadSoil0A && road <= (ushort)TerrainUnitModel.RoadStone8A))
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

        public bool isRiver()
        {
            ushort terrain = getTerrainModel();
            if (terrain >= (ushort)TerrainUnitModel.River0A && terrain <= (ushort)TerrainUnitModel.River8A)
                return true;
            else
                return false;
        }

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
