# BLD-Master

> **Warning:** This repository is no longer maintained. Please use [3BLD Stat Tools](https://github.com/helloCube/bld-stat) and [3BLD Scrambler](https://github.com/helloCube/bld-scr).

## Example Usage

3x3x3 cube

```csharp
var cc = new Cube3Class{
    parityLimit = x => x,
    edgeLimit = x => x.CodeLength is >=10 and <= 12 && x.FlipCount == 0,
    cornerLimit = x => x.CodeLength is >=6 and <= 8 && x.TwistCount == 0,
};
cc.Init();

Console.WriteLine($"Probability={cc.probability}"); // Probability=0.8693965505366232

for (int i = 0; i < 10; i++)
{
    var cube = cc.GetCube3();
    System.Console.Write($"{i+1}. ");
    Console.WriteLine(cube.GetScramble());
    // 1. F2 D' R2 D  L2 R2 D2 R2 D' R2 U' B  U' L  D  B2 R' B' R  D2 U  
    // 2. L  U  B' R  U  D  B' R  U2 F' R2 F2 U2 D2 F  R2 U2 F2 D2 F' U  
    // 3. F' U  L' D2 L' U2 L' R2 B2 D2 R  D2 R2 U' B2 R' F' U2 R' B  U  
    // 4. D2 B2 R2 B2 U' L2 U' L2 B2 U  F2 U' R' F' L' B' R' B2 U2 B  D  
    // 5. L2 B2 R' U2 R' U2 F2 L' B2 R2 B2 U2 B  D  L  F' L' B  R' U2 F2 
    // 6. F  R  L2 F  D' F  D' F' D2 B' L2 F  U2 F  R2 B' L2 R  B2       
    // 7. R2 B2 R2 D2 F2 D' U2 B2 R2 D  L2 D' B  R  D' R2 U2 F  D'       
    // 8. L' F2 D2 R2 F' D2 U2 R2 U2 F' D2 U2 R2 D' F' L2 D' R' D2 L' U2 
    // 9. U  F2 U2 D' L  B  L' B2 U2 D2 R2 U2 R' D2 R' U2 F2 L' D' F     
    // 10. B2 R2 F2 L2 D2 F' L2 F2 D2 B' R2 D' F2 D  R' U2 R' D  U
}
```

4x4x4 cube


```csharp
var cc = new Cube4Class{
    wingLimit = x => x.CodeLength == 24,
    cornerLimit = x => x.CodeLength == 0,
    scrambleXCenter = false
};
cc.Init();
Console.WriteLine($"Probability={cc.probability}"); // Probability=0 ...todo

var cube = cc.GetCube4();
Console.WriteLine(cube.GetScramble());
// R'  U'  B2  D'  B2  R2  D'  R2  U'  R2  U   B'  L'  F2  R'  B'  U'  D'  F   L2  Rw2 Fw2 D2  B'  Uw2 D   F   D2  U   Fw2 U2  F   L   Uw2 U2  F'  Uw2 x2
```

## Some statistic results which I just put here but I have lost their source code

### The distribution of the order of Rubik's cube group elements

Given any move sequence, we will eventually return to the initial state if we repeat the sequence enough times. The number of times we need to repeat the sequence is the order of the group element (i.e. the target state by executing the sequence onto a solved cube). The following table shows the distribution of the order of the group elements.

| Order | Count               |
| ---   | ---                 |
| 1     | 1                   |
| 2     | 170911549183        |
| 3     | 33894540622394      |
| 4     | 4346957030144256    |
| 5     | 133528172514624     |
| 6     | 140621059298755526  |
| 7     | 153245517148800     |
| 8     | 294998638981939200  |
| 9     | 55333752398428896   |
| 10    | 34178553690432192   |
| 11    | 44590694400         |
| 12    | 2330232827455554048 |
| 14    | 23298374383021440   |
| 15    | 14385471333209856   |
| 16    | 150731886270873600  |
| 18    | 1371824848124089632 |
| 20    | 151839445189791744  |
| 21    | 39337151559333120   |
| 22    | 927085127270400     |
| 24    | 3293932519796244480 |
| 28    | 97419760907673600   |
| 30    | 1373347158867028224 |
| 33    | 15874019662233600   |
| 35    | 65526218912563200   |
| 36    | 3768152294808760320 |
| 40    | 835897246403788800  |
| 42    | 737199776831097600  |
| 44    | 100120377950208000  |
| 45    | 197329441659727104  |
| 48    | 911497647410380800  |
| 55    | 4854321355161600    |
| 56    | 671205306846412800  |
| 60    | 4199961633799421952 |
| 63    | 264371433705308160  |
| 66    | 404051175250329600  |
| 70    | 210461722916290560  |
| 72    | 1981453794190295040 |
| 77    | 187238109413376000  |
| 80    | 13349383726694400   |
| 84    | 1697725818678067200 |
| 90    | 1764876446897050368 |
| 99    | 104367909135974400  |
| 105   | 232824419423354880  |
| 110   | 4854321355161600    |
| 112   | 128726200221696000  |
| 120   | 1947044011463147520 |
| 126   | 854783686296207360  |
| 132   | 637129677864960000  |
| 140   | 223125413717606400  |
| 144   | 714192029378150400  |
| 154   | 187238109413376000  |
| 165   | 213590139627110400  |
| 168   | 1050269239266508800 |
| 180   | 2320395168471367680 |
| 198   | 759701292082790400  |
| 210   | 1053174509332070400 |
| 231   | 374476218826752000  |
| 240   | 407156203664179200  |
| 252   | 689877080447385600  |
| 280   | 68653973451571200   |
| 315   | 99309879652515840   |
| 330   | 213590139627110400  |
| 336   | 257452400443392000  |
| 360   | 571019888909352960  |
| 420   | 961155628321996800  |
| 462   | 374476218826752000  |
| 495   | 174755568785817600  |
| 504   | 238381852262400000  |
| 630   | 395380140162416640  |
| 720   | 120144453540249600  |
| 840   | 240288907080499200  |
| 990   | 174755568785817600  |
| 1260  | 51490480088678400   |

### Number of x-centers or t-centers out of position in 5bf

| Number | Count            | Probability  | 
| ---    | ---              | ---          |
| 0      | 1                | 3.0800784636748E-16 |
| 1      | 0                | 0 |
| 2      | 240              | 7.39218831281952E-14 |
| 3      | 2560             | 7.88500086700748E-13 |
| 4      | 46620            | 1.43593257976519E-11 |
| 5      | 581376           | 1.7906836968974E-10 |
| 6      | 6629680          | 2.04199345890555E-09 |
| 7      | 64154880         | 1.97602064227641E-08 |
| 8      | 539147295        | 1.66061597207802E-07 |
| 9      | 3927917440       | 1.20982939140366E-06 |
| 10     | 24886550016      | 7.66525267394473E-06 |
| 11     | 137209752960     | 4.22616805098235E-05 |
| 12     | 658128223120     | 0.000202708656636847 |
| 13     | 2741884721280    | 0.00084452200798935 |
| 14     | 9892350759360    | 0.00304692165290218 |
| 15     | 30763384990336   | 0.00947536395784705 |
| 16     | 81910672955415   | 0.0252291299715083 |
| 17     | 185000621132160  | 0.0569816428915627 |
| 18     | 349913277503440  | 0.107776035019221 |
| 19     | 544438016772480  | 0.167691181026673 |
| 20     | 679274481651396  | 0.209221870185833 |
| 21     | 654044867816320  | 0.201450951163808 |
| 22     | 456702275019600  | 0.140667884159915 |
| 23     | 206032439164800  | 0.0634596078689888 |
| 24     | 45131501617225   | 0.0139008566164519 |

Sum = 3246670537110000 = $24!/24^6$

Mean = 20.0

### Probability of edge buffer position in full parity (will explain)

No constraints on the parity:

| Position | Count        | Probability         | Cumulative Probability |
| ---      | ---          | ---                 | ---                    |
| #1       | 490497638400 | 0.5                 | 0.5                    |
| #2       | 165357158400 | 0.168560606060606   | 0.668560606060606      |
| #3       | 82864373760  | 0.084469696969697   | 0.753030303030303      |
| #4       | 50180014080  | 0.0511521464646465  | 0.80418244949495       |
| #5       | 33901056000  | 0.0345578177609428  | 0.838740267255892      |
| #6       | 24637063680  | 0.0251143550459957  | 0.863854622301888      |
| #7       | 18881856000  | 0.0192476523042929  | 0.883102274606181      |
| #8       | 15078452544  | 0.0153705658942475  | 0.898472840500428      |
| #9       | 12448644384  | 0.0126898107242752  | 0.911162651224704      |
| #10      | 10568158788  | 0.0107728946692519  | 0.921935545893956      |
| #11      | 9189703292   | 0.00936773449305154 | 0.931303280387007      |
| Else     | 67391157472  | 0.0686967196129929  | 1                      |

Sum = 980995276800 = $2^{11}\times 12!$

Conditioned on odd parity:

| Position | Count        | Probability         | Cumulative Probability |
| ---      | ---          | ---                 | ---                    |
| #1       | 241532928000 | 0.492424242424242   | 0.492424242424242      |
| #2       | 79705866240  | 0.1625              | 0.654924242424242      |
| #3       | 39109754880  | 0.0797348484848485  | 0.734659090909091      |
| #4       | 23335280640  | 0.0475747053872054  | 0.782233796296296      |
| #5       | 15690101760  | 0.0319881290584416  | 0.814221925354738      |
| #6       | 11487156480  | 0.0234193920229076  | 0.837641317377645      |
| #7       | 8980616064   | 0.0183091932782688  | 0.855950510655914      |
| #8       | 7398461472   | 0.0150835822495165  | 0.871034092905431      |
| #9       | 6357359112   | 0.0129610391861165  | 0.883995132091547      |
| #10      | 5650322210   | 0.0115195706720043  | 0.895514702763552      |
| #11      | 5158302131   | 0.0105164668026259  | 0.906031169566177      |
| Else     | 46091489411  | 0.0939688304338225  | 1                      |

Sum = 490497638400 = $2^{10}\times 12!$

## Acknowledgements

+ Many thanks to [Chen Shuang](https://github.com/cs0x7f/min2phase) for the [min2phase](https://github.com/cs0x7f/min2phase) algorithm for solving 333 cube and [TPR-4x4x4-Solver](https://github.com/cs0x7f/TPR-4x4x4-Solver) for solving 444 cube, which I brute-forcely modified to C# in this project.

## License GPL-3.0

```
Copyright (C) 2019-2025 Xi Lu

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
```
