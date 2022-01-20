using System;
using System.Collections.Generic;
using System.Linq;

namespace ACNHPokerCore
{
    static class ItemAttr
    {
        public const UInt16 empty = 0xFFFE;
        private static readonly HashSet<UInt16> hasDurabilitySet = new HashSet<UInt16>
        {
            0x0833, // shovel
            0x0834, // axe
            0x0948, // flimsy net
            0x0949, // flimsy rod
            0x094b, // watering can
            0x0b06, // slingshot
            0x0bfc, // flimsy axe
            0x0c0a, // flimsy shovel
            0x0c0b, // flimsy watering can
            0x0c0c, // flimsy rod
            0x1698, // net
            0x16e0, // stone axe
            0x1ff0, // star net
            0x1ff1, // colorfulnet
            0x1ff2, // outdoorsy net
            0x1ff3, // golden net
            0x2117, // fish fishing rod
            0x2118, // colorful fishing rod
            0x2119, // outdoorsy fishing rod
            0x2155, // golden watering can
            0x2156, // colorful watering can
            0x2157, // elephant watering can
            0x2158, // outdoorsy watering can
            0x217e, // golden shovel
            0x217f, // outdoorsy shovel
            0x2180, // printed-design shovel
            0x2181, // colorful shovel
            0x2182, // golden slingshot
            0x2183, // outdoorsy slingshot
            0x2184, // colorful slingshot
            0x21d4, // golden rod
            0x2591, // golden axe
            0x3147, // worn axe
        };
        private static readonly HashSet<UInt16> hasUseSet = new HashSet<UInt16>
        {
            0x0144, // rainbow soft serve
            0x0145, // berry-vanilla soft serve
            0x0146, // matcha-vanilla soft serve
            0x0147, // chocolate soft serve
            0x014C, // matcha donut
            0x014E, // strawberry donut
            0x0151, // white-chocolate donut
            0x38CD, // takeout coffee
            0x3924, // boba milk tea
            0x3925, // plain cotton candy
            0x3927, // chocolate frozen treat
            0x3928, // ramune-soda frozen treat
            0x3929, // orange frozen treat
            0x392B, // vanilla cone
            0x392C, // vanilla soft serve
            0x392E, // canned orange juice
            0x3931, // chocolate donut
            0x398E, // ramune-soda-lemon cone
            0x3996,
            0x3997,
            0x3998,
            0x3999,
            0x399A,
            0x399B,
            0x399C,
            0x39B7,
            0x39B8,
            0x39B9,
            0x39BA,
            0x39BB,
            0x39BC,
            0x39BD,
            0x39F4,
            0x39F5,
            0x3A04,
            0x3A05,
        };
        private static readonly HashSet<UInt16> hasGeneticsSet = new HashSet<UInt16>
        {
            0x0A43, // red-cosmos stems
            0x0A44, // red-cosmos buds
            0x0A46, // white-cosmos stems
            0x0A47, // white-cosmos buds
            0x0B31, // yellow-cosmos stems
            0x0B32, // yellow-cosmos buds
            0x0B35, // pink-cosmos stems
            0x0B36, // pink-cosmos buds
            0x0B39, // orange-cosmos stems
            0x0B3A, // orange-cosmos buds
            0x0B3D, // black-cosmos stems
            0x0B3E, // black-cosmos buds
            0x0B41, // white-tulip stems
            0x0B42, // white-tulip buds
            0x0B45, // red-tulip stems
            0x0B46, // red-tulip buds
            0x0B49, // yellow-tulip stems
            0x0B4A, // yellow-tulip buds
            0x0B4D, // pink-tulip stems
            0x0B4E, // pink-tulip buds
            0x0B51, // orange-tulip stems
            0x0B52, // orange-tulip buds
            0x0B55, // purple-tulip stems
            0x0B56, // purple-tulip buds
            0x0B59, // black-tulip stems
            0x0B5A, // black-tulip buds
            0x0B5D, // white-pansy stems
            0x0B5E, // white-pansy buds
            0x0B61, // red-pansy stems
            0x0B62, // red-pansy buds
            0x0B65, // yellow-pansy stems
            0x0B66, // yellow-pansy buds
            0x0B69, // orange-pansy stems
            0x0B6A, // orange-pansy buds
            0x0B6D, // purple-pansy stems
            0x0B6E, // purple-pansy buds
            0x0B71, // blue-pansy stems
            0x0B72, // blue-pansy buds
            0x0B75, // white-rose stems
            0x0B76, // white-rose buds
            0x0B79, // red-rose stems
            0x0B7A, // red-rose buds
            0x0B7D, // yellow-rose stems
            0x0B7E, // yellow-rose buds
            0x0B81, // pink-rose stems
            0x0B82, // pink-rose buds
            0x0B85, // orange-rose stems
            0x0B86, // orange-rose buds
            0x0B89, // purple-rose stems
            0x0B8A, // purple-rose buds
            0x0B8D, // black-rose stems
            0x0B8E, // black-rose buds
            0x0B91, // blue-rose stems
            0x0B92, // blue-rose buds
            0x0B95, // gold-rose stems
            0x0B96, // gold-rose buds
            0x0BA5, // white-lily stems
            0x0BA6, // white-lily buds
            0x0BA9, // red-lily stems
            0x0BAA, // red-lily buds
            0x0BAD, // yellow-lily stems
            0x0BAE, // yellow-lily buds
            0x0BB1, // pink-lily stems
            0x0BB2, // pink-lily buds
            0x0BB5, // orange-lily stems
            0x0BB6, // orange-lily buds
            0x0BB9, // black-lily stems
            0x0BBA, // black-lily buds
            0x0E7F, // white-windflower stems
            0x0E80, // white-windflower buds
            0x0E83, // orange-windflower stems
            0x0E84, // orange-windflower buds
            0x0E86, // blue-windflower stems
            0x0E87, // blue-windflower buds
            0x0E89, // pink-windflower stems
            0x0E8A, // pink-windflower buds
            0x0E8D, // red-windflower stems
            0x0E8E, // red-windflower buds
            0x0E90, // purple-windflower stems
            0x0E91, // purple-windflower buds
            0x0E94, // white-hyacinth stems
            0x0E95, // white-hyacinth buds
            0x0E98, // yellow-hyacinth stems
            0x0E98, // yellow-hyacinth buds
            0x0E9B, // pink-hyacinth stems
            0x0E9C, // pink-hyacinth buds
            0x0E9E, // orange-hyacinth stems
            0x0E9F, // orange-hyacinth buds
            0x0EA2, // red-hyacinth stems
            0x0EA3, // red-hyacinth buds
            0x0EA5, // blue-hyacinth stems
            0x0EA6, // blue-hyacinth buds
            0x0EA8, // purple-hyacinth stems
            0x0EA9, // purple-hyacinth buds
            0x0EAC, // white-mum stems
            0x0EAD, // white-mum buds
            0x0EB0, // yellow-mum stems
            0x0EB1, // yellow-mum buds
            0x0EB3, // purple-mum stems
            0x0EB4, // purple-mum buds
            0x0EB6, // pink-mum stems
            0x0EB7, // pink-mum buds
            0x0EBA, // red-mum stems
            0x0EBB, // red-mum buds
            0x0EC2, // green-mum stems
            0x0EC3, // green-mum buds
            0x0EEF, // red-cosmos sprouts
            0x0EF0, // red-cosmos plant
            0x0EF1, // white-cosmos sprouts
            0x0EF2, // white-cosmos plant
            0x0EF3, // yellow-cosmos sprouts
            0x0EF4, // yellow-cosmos plant
            0x0EF6, // pink-cosmos plant
            0x0EF8, // orange-cosmos plant
            0x0EFA, // black-cosmos plant
            0x0EFB, // white-tulip sprouts
            0x0EFC, // white-tulip plant
            0x0EFD, // red-tulip sprouts
            0x0EFE, // red-tulip plant
            0x0EFF, // yellow-tulip sprouts
            0x0F00, // yellow-tulip plant
            0x0F02, // pink-tulip plant
            0x0F04, // orange-tulip plant
            0x0F06, // purple-tulip plant
            0x0F08, // black-tulip plant
            0x0F09, // white-pansy sprouts
            0x0F0A, // white-pansy plant
            0x0F0B, // red-pansy sprouts
            0x0F0C, // red-pansy plant
            0x0F0D, // yellow-pansy sprouts
            0x0F0E, // yellow-pansy plant
            0x0F10, // orange-pansy plant
            0x0F12, // purple-pansy plant
            0x0F14, // blue-pansy plant
            0x0F15, // white-rose sprouts
            0x0F16, // white-rose plant
            0x0F17, // red-rose sprouts
            0x0F18, // red-rose plant
            0x0F19, // yellow-rose sprouts
            0x0F1A, // yellow-rose plant
            0x0F1C, // pink-rose plant
            0x0F1E, // orange-rose plant
            0x0F20, // purple-rose plant
            0x0F22, // black-rose plant
            0x0F24, // blue-rose plant
            0x0F26, // gold-rose plant
            0x0F2D, // white-lily sprouts
            0x0F2E, // white-lily plant
            0x0F2F, // red-lily sprouts
            0x0F30, // red-lily plant
            0x0F31, // yellow-lily sprouts
            0x0F32, // yellow-lily plant
            0x0F34, // pink-lily plant
            0x0F36, // orange-lily plant
            0x0F38, // black-lily plant
            0x0F39, // white-windflower sprouts
            0x0F3A, // white-windflower plant
            0x0F3B, // orange-windflower sprouts
            0x0F3C, // orange-windflower plant
            0x0F3E, // blue-windflower plant
            0x0F40, // pink-windflower plant
            0x0F41, // red-windflower sprouts
            0x0F42, // red-windflower plant
            0x0F44, // purple-windflower plant
            0x0F45, // white-hyacinth sprouts
            0x0F46, // white-hyacinth plant
            0x0F47, // yellow-hyacinth sprouts
            0x0F48, // yellow-hyacinth plant
            0x0F4A, // pink-hyacinth plant
            0x0F4C, // orange-hyacinth plant
            0x0F4D, // red-hyacinth sprouts
            0x0F4E, // red-hyacinth plant
            0x0F50, // blue-hyacinth plant
            0x0F52, // purple-hyacinth plant
            0x0F53, // white-mum sprouts
            0x0F54, // white-mum plant
            0x0F55, // yellow-mum sprouts
            0x0F56, // yellow-mum plant
            0x0F58, // purple-mum plant
            0x0F5A, // pink-mum plant
            0x0F5B, // red-mum sprouts
            0x0F5C, // red-mum plant
            0x0F5E, // green-mum plant
            0x1DE3, // lily-of-the-valley plant
            0xEC62, //red-cosmos (Sprout)
            0xEA6F, //red-cosmos (Stem)
            0xEA70, //red-cosmos (Bud)
            0xEA71, //red-cosmos (Flower)
            0xEC64, //yellow-cosmos (Sprout)
            0xEA89, //yellow-cosmos (Stem)
            0xEA8A, //yellow-cosmos (Bud)
            0xEA8B, //yellow-cosmos (Flower)
            0xEC65, //white-cosmos (Sprout)
            0xEA72, //white-cosmos (Stem)
            0xEA73, //white-cosmos (Bud)
            0xEA74, //white-cosmos (Flower)
            0xEAFC, //orange-cosmos (Stem)
            0xEAFD, //orange-cosmos (Bud)
            0xEAF8, //orange-cosmos (Flower)
            0xEAFE, //pink-cosmos (Stem)
            0xEAFF, //pink-cosmos (Bud)
            0xEAFA, //pink-cosmos (Flower)
            0xEAFB, //black-cosmos (Stem)
            0xEAF7, //black-cosmos (Bud)
            0xEAF9, //black-cosmos (Flower)
            0xEC69, //white-tulip (Sprout)
            0xEB3D, //white-tulip (Stem)
            0xEB3E, //white-tulip (Bud)
            0xEB3F, //white-tulip (Flower)
            0xEC63, //red-tulip (Sprout)
            0xEB40, //red-tulip (Stem)
            0xEB41, //red-tulip (Bud)
            0xEB42, //red-tulip (Flower)
            0xEC4A, //yellow-tulip (Sprout)
            0xEB43, //yellow-tulip (Stem)
            0xEB44, //yellow-tulip (Bud)
            0xEB45, //yellow-tulip (Flower)
            0xEB46, //pink-tulip (Stem)
            0xEB47, //pink-tulip (Bud)
            0xEB48, //pink-tulip (Flower)
            0xEB49, //orange-tulip (Stem)
            0xEB4A, //orange-tulip (Bud)
            0xEB4B, //orange-tulip (Flower)
            0xEB4C, //purple-tulip (Stem)
            0xEB4D, //purple-tulip (Bud)
            0xEB4E, //purple-tulip (Flower)
            0xEB4F, //black-tulip (Stem)
            0xEB50, //black-tulip (Bud)
            0xEB51, //black-tulip (Flower)
            0xEC50, //white-pansy (Sprout)
            0xEB52, //white-pansy (Stem)
            0xEB53, //white-pansy (Bud)
            0xEB54, //white-pansy (Flower)
            0xEC51, //red-pansy (Sprout)
            0xEB55, //red-pansy (Stem)
            0xEB56, //red-pansy (Bud)
            0xEB57, //red-pansy (Flower)
            0xEC52, //yellow-pansy (Sprout)
            0xEB58, //yellow-pansy (Stem)
            0xEB59, //yellow-pansy (Bud)
            0xEB5A, //yellow-pansy (Flower)
            0xEB5B, //orange-pansy (Stem)
            0xEB5C, //orange-pansy (Bud)
            0xEB5D, //orange-pansy (Flower)
            0xEB5E, //purple-pansy (Stem)
            0xEB5F, //purple-pansy (Bud)
            0xEB60, //purple-pansy (Flower)
            0xEB61, //blue-pansy (Stem)
            0xEB62, //blue-pansy (Bud)
            0xEB63, //blue-pansy (Flower)
            0xEC56, //white-rose (Sprout)
            0xEB64, //white-rose (Stem)
            0xEB65, //white-rose (Bud)
            0xEB66, //white-rose (Flower)
            0xEC57, //red-rose (Sprout)
            0xEB67, //red-rose (Stem)
            0xEB68, //red-rose (Bud)
            0xEB69, //red-rose (Flower)
            0xEC58, //yellow-rose (Sprout)
            0xEB6A, //yellow-rose (Stem)
            0xEB6B, //yellow-rose (Bud)
            0xEB6C, //yellow-rose (Flower)
            0xEB6D, //pink-rose (Stem)
            0xEB6E, //pink-rose (Bud)
            0xEB6F, //pink-rose (Flower)
            0xEB70, //orange-rose (Stem)
            0xEB71, //orange-rose (Bud)
            0xEB72, //orange-rose (Flower)
            0xEB73, //purple-rose (Stem)
            0xEB74, //purple-rose (Bud)
            0xEB75, //purple-rose (Flower)
            0xEB76, //black-rose (Stem)
            0xEB77, //black-rose (Bud)
            0xEB78, //black-rose (Flower)
            0xEB79, //blue-rose (Stem)
            0xEB7A, //blue-rose (Bud)
            0xEB7B, //blue-rose (Flower)
            0xEB7C, //gold-rose (Stem)
            0xEB7D, //gold-rose (Bud)
            0xEB7E, //gold-rose (Flower)
            0xEC60, //white-lily (Sprout)
            0xEB88, //white-lily (Stem)
            0xEB89, //white-lily (Bud)
            0xEB8A, //white-lily (Flower)
            0xEC61, //red-lily (Sprout)
            0xEB8B, //red-lily (Stem)
            0xEB8C, //red-lily (Bud)
            0xEB8D, //red-lily (Flower)
            0xEC5B, //yellow-lily (Sprout)
            0xEB8E, //yellow-lily (Stem)
            0xEB8F, //yellow-lily (Bud)
            0xEB90, //yellow-lily (Flower)
            0xEB91, //pink-lily (Stem)
            0xEB92, //pink-lily (Bud)
            0xEB93, //pink-lily (Flower)
            0xEB94, //orange-lily (Stem)
            0xEB95, //orange-lily (Bud)
            0xEB96, //orange-lily (Flower)
            0xEB97, //black-lily (Stem)
            0xEB98, //black-lily (Bud)
            0xEB99, //black-lily (Flower)
            0xEC42, //red-windflower (Sprout)
            0xEBDF, //red-windflower (Stem)
            0xEBE0, //red-windflower (Bud)
            0xEBDB, //red-windflower (Flower)
            0xEC43, //white-windflower (Sprout)
            0xEC11, //white-windflower (Stem)
            0xEC12, //white-windflower (Bud)
            0xEC13, //white-windflower (Flower)
            0xEC14, //blue-windflower (Stem)
            0xEC18, //blue-windflower (Bud)
            0xEC19, //blue-windflower (Flower)
            0xEC15, //purple-windflower (Stem)
            0xEC1A, //purple-windflower (Bud)
            0xEC1B, //purple-windflower (Flower)
            0xEC16, //pink-windflower (Stem)
            0xEC1C, //pink-windflower (Bud)
            0xEC1D, //pink-windflower (Flower)
            0xEC47, //orange-windflower (Sprout)
            0xEC17, //orange-windflower (Stem)
            0xEC1E, //orange-windflower (Bud)
            0xEC1F, //orange-windflower (Flower)
            0xEC48, //red-mum (Sprout)
            0xEBDD, //red-mum (Stem)
            0xEBDE, //red-mum (Bud)
            0xEBDC, //red-mum (Flower)
            0xEC49, //white-mum (Sprout)
            0xEC04, //white-mum (Stem)
            0xEC08, //white-mum (Bud)
            0xEC09, //white-mum (Flower)
            0xEC3A, //yellow-mum (Sprout)
            0xEC05, //yellow-mum (Stem)
            0xEC0A, //yellow-mum (Bud)
            0xEC0D, //yellow-mum (Flower)
            0xEC06, //purple-mum (Stem)
            0xEC0B, //purple-mum (Bud)
            0xEC0E, //purple-mum (Flower)
            0xEC07, //pink-mum (Stem)
            0xEC0C, //pink-mum (Bud)
            0xEC0F, //pink-mum (Flower)
            0xEC32, //green-mum (Stem)
            0xEC33, //green-mum (Bud)
            0xEC34, //green-mum (Flower)
            0xEC3F, //red-hyacinth (Sprout)
            0xEBE1, //red-hyacinth (Stem)
            0xEBE2, //red-hyacinth (Bud)
            0xEBDA, //red-hyacinth (Flower)
            0xEC3B, //yellow-hyacinth (Sprout)
            0xEBE9, //yellow-hyacinth (Stem)
            0xEBE7, //yellow-hyacinth (Bud)
            0xEBE8, //yellow-hyacinth (Flower)
            0xEC38, //white-hyacinth (Sprout)
            0xEBE6, //white-hyacinth (Stem)
            0xEBE5, //white-hyacinth (Bud)
            0xEBE4, //white-hyacinth (Flower)
            0xEBEF, //purple-hyacinth (Stem)
            0xEBEE, //purple-hyacinth (Bud)
            0xEBED, //purple-hyacinth (Flower)
            0xEBF3, //blue-hyacinth (Stem)
            0xEBF4, //blue-hyacinth (Bud)
            0xEBEA, //blue-hyacinth (Flower)
            0xEBF2, //pink-hyacinth (Stem)
            0xEBF1, //pink-hyacinth (Bud)
            0xEBEB, //pink-hyacinth (Flower)
            0xEBF5, //orange-hyacinth (Stem)
            0xEBF0, //orange-hyacinth (Bud)
            0xEBEC, //orange-hyacinth (Flower)
            0xECC1, //lily-of-the-valley (Flower)
        };
        private static readonly HashSet<UInt16> isRoseSet = new HashSet<UInt16>
        {
            0x0B75, // white-rose stems
            0x0B76, // white-rose buds
            0x0B79, // red-rose stems
            0x0B7A, // red-rose buds
            0x0B7D, // yellow-rose stems
            0x0B7E, // yellow-rose buds
            0x0B81, // pink-rose stems
            0x0B82, // pink-rose buds
            0x0B85, // orange-rose stems
            0x0B86, // orange-rose buds
            0x0B89, // purple-rose stems
            0x0B8A, // purple-rose buds
            0x0B8D, // black-rose stems
            0x0B8E, // black-rose buds
            0x0B91, // blue-rose stems
            0x0B92, // blue-rose buds
            0x0B95, // gold-rose stems
            0x0B96, // gold-rose buds
            0x0F15, // white-rose sprouts
            0x0F16, // white-rose plant
            0x0F17, // red-rose sprouts
            0x0F18, // red-rose plant
            0x0F19, // yellow-rose sprouts
            0x0F1A, // yellow-rose plant
            0x0F1C, // pink-rose plant
            0x0F1E, // orange-rose plant
            0x0F20, // purple-rose plant
            0x0F22, // black-rose plant
            0x0F24, // blue-rose plant
            0x0F26, // gold-rose plant
        };
        private static readonly HashSet<UInt16> isTreeSet = new HashSet<UInt16>
        {
            0x0AEC, // small young hardwood
            0x0AED, // medium young hardwood
            0x0AEE, // large young hardwood
            0x0AEF, // hardwood tree
            0x0AF2, // small young cedar
            0x0AF3, // medium young cedar
            0x0AF4, // large young cedar
            0x0AF5, // cedar tree
            0x0AF6, // nursery coconut
            0x0AF7, // small young coconut
            0x0AF8, // medium young coconut
            0x0AF9, // large young coconut
            0x0AFA, // coconut tree
            0x0AFB, // nursery bamboo
            0x0AFC, // small young bamboo
            0x0AFD, // medium young bamboo
            0x0AFE, // large young bamboo
            0x0AFF, // bamboo tree
            0x0D1C, // nursery apple
            0x0D1D, // small young apple
            0x0D1E, // medium young apple
            0x0D1F, // large young apple
            0x0D20, // apple tree
            0x0D21, // nursery orange
            0x0D22, // small young orange
            0x0D23, // medium young orange
            0x0D24, // large young orange
            0x0D25, // orange tree
            0x0D26, // nursery pear
            0x0D27, // small young pear
            0x0D28, // medium young pear
            0x0D29, // large young pear
            0x0D2A, // pear tree
            0x0D2B, // nursery peach
            0x0D2C, // small young peach
            0x0D2D, // medium young peach
            0x0D2E, // large young peach
            0x0D2F, // peach tree
            0x0D31, // nursery cherry
            0x0D32, // small young cherry
            0x0D33, // medium young cherry
            0x0D34, // large young cherry
            0x0D35, // cherry tree
            0x114A, // money tree
            0x1154, // nursery money tree
            0x1155, // small young money tree
            0x1156, // medium young money tree
            0x1157, // large young money tree
            0x141F, // nursery cedar
            0x1420, // nursery hardwood
            0xEA65,	// Oak (Sapling)
            0xEA66,	// Oak (Grow 1)
            0xEA67,	// Oak (Grow 2)
            0xEA68,	// Oak (Grow 3)
            0xEA60,	// Oak (Full-grown)
            0xEC6D,	// Oak Stump (Grow 1)
            0xEC6C,	// Oak Stump (Grow 2)
            0xEC6B,	// Oak Stump (Grow 3)
            0xEA9F,	// Oak Stump (Full-grown)
            0xECBC,	// Fruit Stump (Grow 1)
            0xECBD,	// Fruit Stump (Grow 2)
            0xECBE,	// Fruit Stump (Grow 3)
            0xECBF,	// Fruit Stump (Grow 4)
            0xEAB5,	// Cedar (Sapling)
            0xEAB7,	// Cedar (Grow 1)
            0xEAB6,	// Cedar (Grow 2)
            0xEAB8,	// Cedar (Grow 3)
            0xEA69,	// Cedar (Full-grown)
            0xEC70,	// Cedar Stump (Grow 1)
            0xEC6F,	// Cedar Stump (Grow 2)
            0xEC6E,	// Cedar Stump (Grow 3)
            0xEAB4,	// Cedar Stump (Full-grown)
            0xEABF,	// Palm (Sapling)
            0xEAC1,	// Palm (Grow 1)
            0xEAC0,	// Palm (Grow 2)
            0xEAC2,	// Palm (Grow 3)
            0xEA77,	// Palm (Full-grown)
            0xEC78,	// Palm Stump (Grow 1)
            0xEC79,	// Palm Stump (Grow 2)
            0xEC7A,	// Palm Stump (Grow 3)
            0xEABE,	// Palm Stump (Full-grown)
            0xEAC4,	// Bamboo (Sapling)
            0xEAC6,	// Bamboo (Grow 1)
            0xEAC5,	// Bamboo (Grow 2)
            0xEAC7,	// Bamboo (Grow 3)
            0xEA76,	// Bamboo (Full-grown)
            0xEAC3,	// Bamboo Stump (Full-grown)
            0xEC73,	// Bamboo Stump (Grow 1)
            0xEC72,	// Bamboo Stump (Grow 2)
            0xEC71,	// Bamboo Stump (Grow 3)
            0xEBD3,	// Apple Tree (Sapling)
            0xEBD0,	// Apple Tree (Grow 1)
            0xEBD1,	// Apple Tree (Grow 2)
            0xEBD2,	// Apple Tree (Grow 3)
            0xEA61,	// Apple Tree (Full-grown)
            0xEBC8,	// Orange Tree (Sapling)
            0xEBC9,	// Orange Tree (Grow 1)
            0xEBCA,	// Orange Tree (Grow 2)
            0xEBCB,	// Orange Tree (Grow 3)
            0xEA62,	// Orange Tree (Full-grown)
            0xEBCC,	// Pear Tree (Sapling)
            0xEBCD,	// Pear Tree (Grow 1)
            0xEBCE,	// Pear Tree (Grow 2)
            0xEBCF,	// Pear Tree (Grow 3)
            0xEAC8,	// Pear Tree (Full-grown)
            0xEBC4,	// Peach Tree (Sapling)
            0xEBC5,	// Peach Tree (Grow 1)
            0xEBC6,	// Peach Tree (Grow 2)
            0xEBC7,	// Peach Tree (Grow 3)
            0xEACA,	// Peach Tree (Full-grown)
            0xEBC2,	// Cherry Tree (Sapling)
            0xEBC3,	// Cherry Tree (Grow 1)
            0xEBC1,	// Cherry Tree (Grow 2)
            0xEBC0,	// Cherry Tree (Grow 3)
            0xEAC9,	// Cherry Tree (Full-grown)
            0xEC9A, // Cedar(Deco) A 
            0xEC97, // Cedar(Deco) B
            0xEC9F, // Money Tree(Sapling)
            0xECA0, // Money Tree(Grow 1)
            0xEC9E, // Money Tree(Grow 2)
            0xEC9D, // Money Tree(Grow 3)
            0xEC9C, // Money Tree(Full-grown)
            0xED16, // Tree(Easter Egg)
        };
        private static readonly HashSet<UInt16> isFruitSet = new HashSet<UInt16>
        {
            0x08A5, // apple
            0x08A6, // orange
            0x08ED, // pear
            0x08EE, // peach
            0x08EF, // cherry
            0x08F0, // coconut
        };
        private static readonly HashSet<UInt16> isMushroomSet = new HashSet<UInt16>
        {
            0x0CCC, // elegant mushroom
            0x0CCD, // round mushroom
            0x0CCE, // skinny mushroom
            0x0CCF, // flat mushroom
            0x0CD0, // rare mushroom
        };
        private static readonly HashSet<UInt16> isCraftMaterialSet = new HashSet<UInt16>
        {
            0x09C4, // tree branch
            0x09C5, // bamboo piece
            0x0ACF, // softwood
            0x0AD0, // wood
            0x0AD1, // hardwood
            0x165F, // acorn
            0x1660, // pine cone
            0x1661, // young spring bamboo
            0x1662, // red ornament
            0x1663, // blue ornament
            0x1664, // gold ornament
            0x30DD, // earth egg
            0x30DE, // stone egg
            0x30DF, // leaf egg
            0x30E0, // wood egg
            0x30E1, // sky egg
            0x30E2, // water egg
            0x09C6, // stone
            0x09C9, // gold nugget
            0x09CF, // iron nugget
            0x0C12, // clay
            0x16E3, // cherry-blossom petal
            0x055E, // sea snail
            0x055F, // venus comb
            0x0560, // conch
            0x0563, // sand dollar
            0x0564, // coral
            0x0565, // giant clam
            0x0566, // cowrie
            0x175E, // summer shell
            0x0DD3, // snowflake
            0x0DD4, // large snowflake
            0x175F, // star fragment
            0x1760, // large star fragment
            0x1761, // Capricorn fragment
            0x1762, // Aquarius fragment
            0x1763, // Pisces fragment
            0x1764, // Aries fragment
            0x1765, // Taurus fragment
            0x1766, // Gemini fragment
            0x1767, // Cancer fragment
            0x1768, // Leo fragment
            0x1769, // Virgo fragment
            0x176A, // Libra fragment
            0x176B, // Scorpius fragment
            0x176C, // Sagittarius fragment
            0x0A40, // clump of weeds
            0x36AA, // flour
            0x36AC, // whole-wheat flour
            0x36AD, // sugar
            0x36AE, // brown sugar
        };
        private static readonly HashSet<UInt16> isDummyContainerSet = new HashSet<UInt16>
        {
            0x1225, // delivery
            0x1180, // present
            0x1095, // delivery box
            0x1E13, // present
            0x1E14, // present
            0x1E15, // present
            0x1E16, // present
            0x1E17, // present
            0x1E18, // present
            0x1E19, // present
            0x1E1A, // present
            0x1E1B, // present
            0x1E1C, // present
            0x1E1D, // present
            0x1E1E, // present
            0x1E1F, // present
            0x1E20, // present
            0x1E21, // present
            0x1E22, // present
        };
        private static readonly HashSet<UInt16> isFenceSet = new HashSet<UInt16>
        {
            0x0C08, // brick fence
            0x0D4A, // vertical-board fence
            0x0D4B, // bamboo lattice fence
            0x10FD, // corral fence
            0x10FE, // country fence
            0x1100, // rope fence
            0x1102, // imperial fence
            0x1104, // straw fence
            0x1105, // iron fence
            0x1106, // spiky fence
            0x1456, // iron-and-stone fence
            0x1457, // zen fence
            0x145C, // stone fence
            0x145D, // barbed-wire fence
            0x2DBF, // simple wooden fence
            0x2DC0, // lattice fence
            0x2FFD, // コミューン島専用柵
            0x3156, // Bunny Day fence
            0x31D6, // hedge
            0x325E, // wedding fence
            0x33DB, // Spooky Fence,
            0x37C6, // mermaid fence
            0x10FF,
            0x1103,
            0x1458,
            0x145A,
            0x39A3,
            0x39A4,
            0x39A5,
            0x39A6,
            0x39A7,


            /*
                0xECC2, // simple wooden fence
                0xEB00, // brick fence
                0xEBBA, // vertical-board fence
                0xEBBF, // bamboo lattice fence
                0xECC3, // lattice fence
                0xEBD5, // corral fence
                0xEBD6, // country fence
                0xEBD8, //; rope fence
                0xEC74, // imperial fence
                0xEC7C, // straw fence
                0xEC7D, // iron fence
                0xEC7E, // spiky fence
                0xEC9B, // stone fence
                0xECB2, // barbed-wire fence
                0xECB6, // zen fence
                0xECB7, // iron-and-stone fence
                0xED0C, // Bunny Day fence
                0xED0B, // wedding fence(Green)
                0xED0D, // wedding fence
                0xED0E, // wedding fence(Pink)
                0xED08, // hedge
                0xED21, // mermaid fence
                0xED25, // spooky fence
                0xECC4, // Harvey's island fence
            */
        };
        private static readonly HashSet<UInt16> isFenceWithVariation = new HashSet<UInt16>{
            0x0D4A, // vertical-board fence
            0x1105, // iron fence
            0x1457, // zen fence
            0x1458, // large lattice fence
            0x2DBF, // simple wooden fence
            0x2DC0, // lattice fence
            0x325E, // wedding fence
            0x39A4, // frozen fence
            0x39A6, // corrugated iron fence
            0x39A7, // park fence
        };
        private static readonly HashSet<UInt16> isFlowerSet = new HashSet<UInt16>
        {
            0x0900, // red cosmos
            0x0901, // white cosmos
            0x0B2F, // yellow cosmos
            0x0B33, // pink cosmos
            0x0B37, // orange cosmos
            0x0B3B, // black cosmos
            0x0B3F, // white tulips
            0x0B43, // red tulips
            0x0B47, // yellow tulips
            0x0B4B, // pink tulips
            0x0B4F, // orange tulips
            0x0B53, // purple tulips
            0x0B57, // black tulips
            0x0B5B, // white pansies
            0x0B5F, // red pansies
            0x0B63, // yellow pansies
            0x0B67, // orange pansies
            0x0B6B, // purple pansies
            0x0B6F, // blue pansies
            0x0B73, // white roses
            0x0B77, // red roses
            0x0B7B, // yellow roses
            0x0B7F, // pink roses
            0x0B83, // orange roses
            0x0B87, // purple roses
            0x0B8B, // black roses
            0x0B8F, // blue roses
            0x0B93, // gold roses
            0x0BA3, // white lilies
            0x0BA7, // red lilies
            0x0BAB, // yellow lilies
            0x0BAF, // pink lilies
            0x0BB3, // orange lilies
            0x0BB7, // black lilies
            0x0E7D, // white windflowers
            0x0E81, // orange windflowers
            0x0E85, // blue windflowers
            0x0E88, // pink windflowers
            0x0E8B, // red windflowers
            0x0E8F, // purple windflowers
            0x0E92, // white hyacinths
            0x0E96, // yellow hyacinths
            0x0E9A, // pink hyacinths
            0x0E9D, // orange hyacinths
            0x0EA0, // red hyacinths
            0x0EA4, // blue hyacinths
            0x0EA7, // purple hyacinths
            0x0EAA, // white mums
            0x0EAE, // yellow mums
            0x0EB2, // purple mums
            0x0EB5, // pink mums
            0x0EB8, // red mums
            0x1437, // green mums
            0x11AD, // Orange Pumpkin
            0x11AE, // Yellow Pumpkin
            0x11AF, // Green Pumpkin
            0x11B0, // White Pumpkin
        };
        private static readonly HashSet<UInt16> isFlowerSeedSet = new HashSet<UInt16>
        {
            0x0A42, // red-cosmos bag
            0x0A45, // white-cosmos bag
            0x0B30, // yellow-cosmos bag
            0x0B40, // white-tulip bag
            0x0B44, // red-tulip bag
            0x0B48, // yellow-tulip bag
            0x0B5C, // white-pansy bag
            0x0B60, // red-pansy bag
            0x0B64, // yellow-pansy bag
            0x0B74, // white-rose bag
            0x0B78, // red-rose bag
            0x0B7C, // yellow-rose bag
            0x0BA4, // white-lily bag
            0x0BA8, // red-lily bag
            0x0BAC, // yellow-lily bag
            0x0E7E, // white-windflower bag
            0x0E82, // orange-windflower bag
            0x0E8C, // red-windflower bag
            0x0E93, // white-hyacinth bag
            0x0E97, // yellow-hyacinth bag
            0x0EA1, // red-hyacinth bag
            0x0EAB, // white-mum bag
            0x0EAF, // yellow-mum bag
            0x0EB9, // red-mum bag
            0x33AB, // Pumpkin Start
            0x0A59, // tomato start
            0x0BD6, // wheat start
            0x0BDB, // sugarcane start
            0x0BE0, // seed potato
            0x0BE5, // arrot start
        };
        private static readonly HashSet<UInt16> isBushStartSet = new HashSet<UInt16>
        {
            0x0B04, // white-azalea start
            0x0BCB, // red-hibiscus start
            0x0BD0, // holly start
            0x0ECA, // pink-azalea start
            0x0ECE, // yellow-hibiscus start
            0x0ED2, // pink-hydrangea start
            0x0ED6, // blue-hydrangea start
            0x30BC, // yellow-tea-olive start
            0x30C0, // orange-tea-olive start
            0x30C7, // red-camellia start
            0x30CA, // pink-camellia start
            0x33CE, // pink-plumeria start
            0x3424, // white-plumeria start

        };
        private static readonly HashSet<UInt16> isWrappingPaperSet = new HashSet<UInt16>
        {
            0x1E03, // yellow wrapping paper
            0x1E04, // pink wrapping paper
            0x1E05, // orange wrapping paper
            0x1E06, // chartreuse wrapping paper
            0x1E07, // green wrapping paper
            0x1E08, // mint wrapping paper
            0x1E09, // light-blue wrapping paper
            0x1E0A, // purple wrapping paper
            0x1E0B, // navy wrapping paper
            0x1E0C, // blue wrapping paper
            0x1E0D, // white wrapping paper
            0x1E0E, // red wrapping paper
            0x1E0F, // gold wrapping paper
            0x1E10, // brown wrapping paper
            0x1E11, // gray wrapping paper
            0x1E12, // black wrapping paper
            0x35E0, // festive wrapping paper
        };
        private static readonly HashSet<UInt16> hasQuantitySet = new HashSet<UInt16>(
            new UInt16[] {
                0x09D1, // Customization Kit
                0X11C5, // Fish Bait
                0X0CBF, // Wasp Nest
                0X094D, // communicator part
                0X0955, // rusted part
                0X0AD9, // medicine
                0X16DB, // Nook Miles Ticket
                0X26EC, // Bell voucher
                0X242D, // Saharah Ticket
                0X172C, // Tailor's Ticket
                0X0AC1, // party popper
                0X0A12, // pitfall seed
                0X0AEA, // Sapling
                0X0AF0, // Cedar Sapling
                0X08F7, // bamboo shoot
                0X125E, // Wisp spirit piece
                0X1CCE, // Maple leaves
                0X3102, // Heart crystals
                0X0ADB, // Birthday cupcake
                0X3331, // communicator
                0X32A8, // pearl
                0X166F, // red sparkler
                0X1122, // bubble blower
                0X32AE, // blue sparkler
                0X33AC, // Dream Bell exchange ticket
                0x338C, // Lolipop
                0x338D, // Candy
                0x0DD5, // FeatherRed ; red feather; あかいはね ; 紅色羽毛 ; 红色羽毛 ; 빨간색 깃털; plume rouge; Rubinfeder ; pluma roja; piuma rossa; rode veer; красное перо; 
                0x0DD6,// FeatherBlue ; blue feather; あおいはね ; 藍色羽毛 ; 蓝色羽毛 ; 파란색 깃털; plume bleue; Azurfeder ; pluma azul; piuma blu; blauwe veer; синее перо; 
                0x0DD7,// FeatherGreen ; green feather; みどりのはね ; 綠色羽毛 ; 绿色羽毛 ; 초록색 깃털; plume verte; Smaragdfeder ; pluma verde; piuma verde; groene veer; зеленое перо; 
                0x0DD9,// FeatherPurple ; purple feather; むらさきのはね ; 紫色羽毛 ; 紫色羽毛 ; 보라색 깃털; plume violette; Lilafeder ; pluma morada; piuma viola; paarse veer; фиолетовое перо; 
                        0x0A0A, // tomato
                        0x0BD5, // wheat
                        0x0BDA, // sugarcane
                        0x0BDF, // potato
                        0x0BE4, // carrot
                        0x3106, // glowing moss
                        0x3107, // vine
                        0x3684, // otoshidama envelope
    }
            .Union(isCraftMaterialSet)
            .Union(isFenceSet)
            .Union(isFlowerSet)
            .Union(isFlowerSeedSet)
            .Union(isBushStartSet)
            .Union(isFruitSet)
            .Union(isMushroomSet)
            .Union(isWrappingPaperSet)
            );
        private static readonly HashSet<UInt16> containsItemSet = new HashSet<UInt16>(
            new UInt16[] {
                0x0A13, // Fossils contain the fossil
            }
            .Union(isTreeSet)
            .Union(isDummyContainerSet)
            );
        private static readonly HashSet<UInt16> isShellSet = new HashSet<UInt16>(
            new UInt16[] {
                0x0560,
                0x0564,
                0x055E,
                0x0563,
                0x055F,
                0x0565,
                0x0566,
                0x175E,
            }
        );
        private static readonly HashSet<UInt16> isWeedSet = new HashSet<UInt16>(
            new UInt16[] {
                0xEB28,	// Spring Weed 1A
                0xEB30,	// Spring Weed 1B
                0xEB31,	// Spring Weed 1C
                0xEB32,	// Spring Weed 2A
                0xEB33,	// Spring Weed 2B
                0xEB34,	// Spring Weed 2C
                0xEB35,	// Spring Weed 3
                0xEA86,	// Summer Weed 1A
                0xEA87,	// Summer Weed 1B
                0xEA88,	// Summer Weed 1C
                0xEB02,	// Summer Weed 2A
                0xEB04,	// Summer Weed 2B
                0xEB03,	// Summer Weed 2C
                0xEB01,	// Summer Weed 3
                0xEA81,	// Autumn Weed 01A
                0xEA82,	// Autumn Weed 01B
                0xEA83,	// Autumn Weed 01C
                0xEB19,	// Autumn Weed 02A
                0xEB17,	// Autumn Weed 02B
                0xEB18,	// Autumn Weed 02C
                0xEB05,	// Autumn Weed 03
                0xEC8C,	// Autumn Weed 11A
                0xEC86,	// Autumn Weed 11B
                0xEC87,	// Autumn Weed 11C
                0xEC88,	// Autumn Weed 12A
                0xEC89,	// Autumn Weed 12B
                0xEC8A,	// Autumn Weed 12C
                0xEC8B,	// Autumn Weed 13
                0xEC83,	// Autumn Weed 21A
                0xEC84,	// Autumn Weed 21B
                0xEC85,	// Autumn Weed 21C
                0xEC81,	// Autumn Weed 22A
                0xEC82,	// Autumn Weed 22B
                0xEC80,	// Autumn Weed 22C
                0xEC7F,	// Autumn Weed 23
                0xEB36,	// Winter Weed 01A
                0xEB37,	// Winter Weed 01B
                0xEB38,	// Winter Weed 01C
                0xEB39,	// Winter Weed 02A
                0xEB3A,	// Winter Weed 02B
                0xEB3B,	// Winter Weed 02C
                0xEB3C,	// Winter Weed 03
                0xEC91,	// Winter Weed 11A
                0xEC92,	// Winter Weed 11B
                0xEC93,	// Winter Weed 11C
                0xEC8F,	// Winter Weed 12A
                0xEC90,	// Winter Weed 12B
                0xEC8E,	// Winter Weed 12C
                0xEC8D,	// Winter Weed 13
            }
        );
        private static readonly HashSet<UInt16> isStoneSet = new HashSet<UInt16>(
            new UInt16[] {
                0xEB12, // Stone A
                0xEB13, // Stone B
                0xEB16, // Stone C
                0xEB15, // Stone D
                0xEB14, // Stone E
            }
        );

        private static readonly HashSet<UInt16> isBushSet = new HashSet<UInt16>(
        new UInt16[] {
                0xEACC,//PltBushAzalea0White,//white-azalea(Sapling); 00000000
                0xEAD0,//PltBushAzalea1White,//white-azalea(Grow 1); 00000000
                0xEACF,//PltBushAzalea2White,//white-azalea(Bush); 00000000
                0xEC21,//PltBushAzalea0Pink,//pink-azalea(Sapling); 00000000
                0xEC20,//PltBushAzalea1Pink,//pink-azalea(Grow 1); 00000000
                0xEC24,//PltBushAzalea2Pink,//pink-azalea(Bush); 00000000
                0xEBF9,//PltBushHydrangea,//blue-hydrangea(Sapling); 00000000
                0xEBFC,//PltBushHydrangea1Blue,//blue-hydrangea(Grow 1); 00000000
                0xEBFF,//PltBushHydrangea2Blue,//blue-hydrangea(Bush); 00000000
                0xEBFB,//PltBushHydrangea0Pink,//pink-hydrangea(Sapling); 00000000
                0xEC00,//PltBushHydrangea1Pink,//pink-hydrangea(Grow 1); 00000000
                0xEC03,//PltBushHydrangea2Pink,//pink-hydrangea(Bush); 00000000
                0xEAD2,//PltBushHibiscus0Red,//red-hibiscus(Sapling); 00000000
                0xEAD5,//PltBushHibiscus1Red,//red-hibiscus(Grow 1); 00000000
                0xEAD4,//PltBushHibiscus2Red,//red-hibiscus(Bush); 00000000
                0xEC25,//PltBushHibiscus0Yellow,//yellow-hibiscus(Sapling); 00000000
                0xEC26,//PltBushHibiscus1Yellow,//yellow-hibiscus(Grow 1); 00000000
                0xEC29,//PltBushHibiscus2Yellow,//yellow-hibiscus(Bush); 00000000
                0xEAD7,//PltBushHolly0,//holly(Sapling); 00000000
                0xEAD9,//PltBushHolly1,//holly(Grow 1); 00000000
                0xEADA,//PltBushHolly2,//holly(Bush); 00000000
                0xECF8,//PltBushOsmathus1Yello,//yellow-tea-olive(Sapling); 00000000
                0xECF6,//PltBushOsmathus2Yello,//yellow-tea-olive(Grow 1); 00000000
                0xECF4,//PltBushOsmathus3Yello,//yellow-tea-olive(Bush); 00000000
                0xECFC,//PltBushOsmathus1Orange,//orange-tea-olive(Sapling); 00000000
                0xECFD,//PltBushOsmathus2Orange,//orange-tea-olive(Grow 1); 00000000
                0xECF9,//PltBushOsmathus3Orange,//orange-tea-olive(Bush); 00000000
                0xED02,//PltBushCamellia1Red,//red-camellia(Sapling); 00000000
                0xED04,//PltBushCamellia2Red,//red-camellia(Grow 1); 00000000
                0xED06,//PltBushCamellia3Red,//red-camellia(Bush); 00000000
                0xECFE,//PltBushCamellia1Pink,//pink-camellia(Sapling); 00000000
                0xECFF,//PltBushCamellia2Pink,//pink-camellia(Grow 1); 00000000
                0xED01,//PltBushCamellia3Pink,//pink-camellia(Bush); 00000000
            }
        );

        public static bool hasDurability(UInt16 item)
        {
            return hasDurabilitySet.Contains(item);
        }
        public static bool hasUse(UInt16 item)
        {
            return hasUseSet.Contains(item);
        }
        public static bool hasQuantity(UInt16 item)
        {
            return hasQuantitySet.Contains(item);
        }
        public static bool hasGenetics(UInt16 item)
        {
            return hasGeneticsSet.Contains(item);
        }
        public static bool isRose(UInt16 item)
        {
            return isRoseSet.Contains(item);
        }
        public static bool isTree(UInt16 item)
        {
            return isTreeSet.Contains(item);
        }
        public static bool containsItem(UInt16 item)
        {
            return containsItemSet.Contains(item);
        }
        public static bool isFlower(UInt16 item)
        {
            return isFlowerSet.Contains(item);
        }
        public static bool isShell(UInt16 item)
        {
            return isShellSet.Contains(item);
        }
        public static bool isWeed(UInt16 item)
        {
            return isWeedSet.Contains(item);
        }
        public static bool isStone(UInt16 item)
        {
            return isStoneSet.Contains(item);
        }
        public static bool isFence(UInt16 item)
        {
            return isFenceSet.Contains(item);
        }
        public static bool isBush(UInt16 item)
        {
            return isBushSet.Contains(item);
        }
        public static bool hasFenceWithVariation(UInt16 item)
        {
            return isFenceWithVariation.Contains(item);
        }
    }
}
