using System;
using System.Collections.Generic;
using System.Linq;

namespace Luxi {
    class XCenterCC {
        private static readonly long[] Counts = { 1, 0, 240, 2560, 46620, 581376, 6629680, 64154880,
            539147295, 3927917440, 24886550016, 137209752960, 658128223120, 2741884721280, 9892350759360,
            30763384990336, 81910672955415, 185000621132160, 349913277503440, 544438016772480,
            679274481651396, 654044867816320, 456702275019600, 206032439164800, 45131501617225 };
        public const long Sum = 3246670537110000;
        public static XCenter GetInstance(bool scramble){
            return scramble ? XCenter.Random() : new XCenter();
        }
    }
}