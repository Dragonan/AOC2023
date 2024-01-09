using System.Numerics;

namespace AOC2023
{
    public struct PointL
    {
        public PointL(long x, long y, long z = 0)
        {
            X = x;
            Y = y;
            Z = Z;
        }

        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
    }

    public class Hailstone
    {
        public Hailstone(Vector<long> coords, Vector<long> velocity)
        {
            Coords = coords;
            Velocity = velocity;
            EndCoords = Vector<long>.Zero;
            StartCoords = Vector<long>.Zero;
        }

        public Vector<long> Coords { get; }
        public Vector<long> Velocity { get; }
        public long X => Coords[0];
        public long Y => Coords[1];
        public long Z => Coords[2];
        public long XChange => Velocity[0];
        public long YChange => Velocity[1];
        public long ZChange => Velocity[2];
        public Vector<long> StartCoords { get; set; }
        public long XStart => StartCoords[0];
        public long YStart => StartCoords[1];
        public long ZStart => StartCoords[2];
        public Vector<long> EndCoords { get; set; }
        public long XEnd => EndCoords[0];
        public long YEnd => EndCoords[1];
        public long ZEnd => EndCoords[2];
        public bool SegmentCalculated { get; set; }
        public bool IsOutOfBounds { get; set; }
        public int InterceptCount { get; set; }
    }

    public static class Day24
    {
        static List<Hailstone> Hailstones = new List<Hailstone>();
        public static void Solve()
        {
            var rows = input.Split('\n').ToArray();
            ParseHailstones(rows);

            IsEnoughDakkaPossible();
            return;


            var count = GetCollidedHailstonesForPart1();
            Console.WriteLine(count);
            Console.WriteLine(Hailstones.Max(h => h.InterceptCount));
            Console.WriteLine(Hailstones.Count(h => h.IsOutOfBounds));
        }

        static bool IsEnoughDakkaPossible()
        {
            UInt128 dakka = 0;
            var max = 60;//0000000000000;
            for (int x1 = 0; x1 < max; x1++)       
            {
                for (int y1 = 0; y1 < max; y1++)
                {
                    for (int x2 = 0; x2 < max; x2++)
                    {
                        for (int y2 = 0; y2 < max; y2++)
                        {
                            foreach (var hailstone in Hailstones)
                            {
                                var x = hailstone.X/10000000000000;//0;
                                var y = hailstone.Y/10000000000000;//0;
                                var minX = x1 <= x2 ? x1 : x2;
                                var maxX = x1 > x2 ? x1 : x2;
                                var minY = y1 <= y2 ? y1 : y2;
                                var maxY = y1 > y2 ? y1 : y2;
                                if ((x < minX && hailstone.XChange <= 0) ||
                                    (x > maxX && hailstone.XChange >= 0) ||
                                    (y < minY && hailstone.YChange <= 0) ||
                                    (y > maxY && hailstone.YChange >= 0))
                                    continue;
                                dakka++;
                            }
                        }
                    }
                }
            }

            Console.WriteLine(dakka);

            return true;
        }

        static Hailstone FindThePerfectShotForPart2()
        {
            var min = 000000000000000;
            var max = 600000000000000;
            var simplifier = 1000000;

            var start = new PointL(min,min,min);
            for (int x = min; x < max; x++)
                for (int y = min; y < max; y++)
                {
                    var end = new PointL(x,y,max);
                    foreach (var hailstone in Hailstones)
                    {
                        
                    }

                }

            return new Hailstone(Vector<long>.Zero, Vector<long>.Zero);
        }


        static int GetCollidedHailstonesForPart1()
        {
            var min = 200000000000000;
            var max = 400000000000000;
            // var min = 000000000000000;
            // var max = 600000000000000;
            var simplifier = 1000000;
            var count = 0;
            
            for (int a = 0; a < Hailstones.Count-1; a++)
            {
                var aSegment = Hailstones[a];
                if (aSegment.IsOutOfBounds || (aSegment.IsOutOfBounds = IsHailstoneOutOfBounds(aSegment,min,max)))
                    continue;

                FindSegment(aSegment, min, max);
                if (aSegment.IsOutOfBounds)
                    continue;

                for (int b = a+1; b < Hailstones.Count; b++)
                {
                    var bSegment = Hailstones[b];
                    if (bSegment.IsOutOfBounds || (bSegment.IsOutOfBounds = IsHailstoneOutOfBounds(bSegment,min,max)))
                        continue;

                    FindSegment(bSegment, min, max);
                    if (bSegment.IsOutOfBounds)
                        continue;

                    //if (ExtraMath.DoIntersect(new PointL(aSegment.XStart/simplifier, aSegment.YStart/simplifier), new PointL(aSegment.XEnd/simplifier, aSegment.YEnd/simplifier),
                                              //new PointL(bSegment.XStart/simplifier, bSegment.YStart/simplifier), new PointL(bSegment.XEnd/simplifier, bSegment.YEnd/simplifier)))
                    if (DoLinesIntersect(aSegment.StartCoords, aSegment.EndCoords, bSegment.StartCoords, bSegment.EndCoords))
                    {
                        count++;
                        aSegment.InterceptCount++;
                        bSegment.InterceptCount++;
                    }
                }
            }

            return count;
        }

        static bool DoLinesIntersect(Vector<long> a, Vector<long> b, Vector<long> c, Vector<long> d)
        {
            var simplifier = 1000000;
            Int128 a1 = a[0]/simplifier;
            Int128 a2 = a[1]/simplifier;
            Int128 a3 = a[2]/simplifier;
            Int128 b1 = b[0]/simplifier;
            Int128 b2 = b[1]/simplifier;
            Int128 b3 = b[2]/simplifier;
            Int128 c1 = c[0]/simplifier;
            Int128 c2 = c[1]/simplifier;
            Int128 c3 = c[2]/simplifier;
            Int128 d1 = d[0]/simplifier;
            Int128 d2 = d[1]/simplifier;
            Int128 d3 = d[2]/simplifier;
            var A=b1-a1;
            var B=c1-d1;
            var C=c1-a1;
            var D=b2-a2;
            var E=c2-d2;
            var F=c2-a2;

            //find t and s using formula
            var t=(C*E-F*B)/(E*A-B*D);
            var s=(D*C-A*F)/(D*B-A*E);

            //check if third equation is also satisfied(we have 3 equations and 2 variable
            //if ((t*(b3-a3)+s*(c3-d3))==c3-a3)
                if(0<=t && t<=1 && 0<=s && s<=1)
                return true;
            return false;
        }

        static bool IsHailstoneOutOfBounds(Hailstone hailstone, long min, long max)
        {
            return (hailstone.X < min && hailstone.XChange <= 0) || 
                    (hailstone.X > max && hailstone.XChange >= 0) ||
                    (hailstone.Y < min && hailstone.YChange <= 0) || 
                    (hailstone.Y > max && hailstone.YChange >= 0);
        }

        static void FindSegment(Hailstone hailstone, long min, long max, bool includeZ = false)
        {
            if (hailstone.IsOutOfBounds || hailstone.SegmentCalculated)
                return;

            long xTimeToStart = 0, yTimeToStart = 0, zTimeToStart = 0;
            if (hailstone.X < min || hailstone.X > max)
                xTimeToStart = (long)Math.Abs(Math.Ceiling((Math.Max(min, hailstone.X) - Math.Min(hailstone.X, max))/(double)hailstone.XChange));
            if (hailstone.Y < min || hailstone.Y > max)
                yTimeToStart = (long)Math.Abs(Math.Ceiling((Math.Max(min, hailstone.Y) - Math.Min(hailstone.Y, max))/(double)hailstone.YChange));
            if (hailstone.Z < min || hailstone.Z > max)
                zTimeToStart = (long)Math.Abs(Math.Ceiling((Math.Max(min, hailstone.Z) - Math.Min(hailstone.Z, max))/(double)hailstone.ZChange));
            var timeToStart = Math.Max(xTimeToStart,yTimeToStart);
            if (includeZ)
                timeToStart = Math.Max(timeToStart,zTimeToStart);
            var xStart = hailstone.X + hailstone.XChange*timeToStart;
            var yStart = hailstone.Y + hailstone.YChange*timeToStart;
            var zStart = hailstone.Z + hailstone.ZChange*timeToStart;

            if (xStart < min || (max-hailstone.XChange) < xStart ||
                yStart < min || max-hailstone.YChange < yStart ||
                (includeZ && (zStart < min || max-hailstone.ZChange < zStart)))
            {
                hailstone.IsOutOfBounds = true;
                return;
            }

            var xLength = hailstone.XChange >= 0 ? max - xStart : xStart - min;
            var yLength = hailstone.YChange >= 0 ? max - yStart : yStart - min;
            var zLength = hailstone.ZChange >= 0 ? max - zStart : zStart - min;
            var xTimeToEnd = (long)Math.Abs(Math.Ceiling(xLength/(double)hailstone.XChange));
            var yTimeToEnd = (long)Math.Abs(Math.Ceiling(yLength/(double)hailstone.YChange));
            var zTimeToEnd = (long)Math.Abs(Math.Ceiling(zLength/(double)hailstone.ZChange));
            var timeToEnd = Math.Min(xTimeToEnd,yTimeToEnd);
            if (includeZ)
                timeToEnd = Math.Min(timeToEnd,zTimeToEnd);
            var xEnd = xStart + hailstone.XChange*timeToEnd;
            var yEnd = yStart + hailstone.YChange*timeToEnd;
            var zEnd = zStart + hailstone.ZChange*timeToEnd;

            hailstone.StartCoords = new Vector<long>(new [] { xStart, yStart, zStart, 0L });
            hailstone.EndCoords = new Vector<long>(new [] { xEnd, yEnd, zEnd, 0L });
            hailstone.SegmentCalculated = true;
        }

        static void ParseHailstones(string[] rows)
        {
            var zeroArray = new [] { 0L };
            foreach (var row in rows)
            {
                var data = row.Split('@');
                var coords = data[0].Trim().Split(", ").Select(long.Parse).Concat(zeroArray).ToArray();
                var velocity = data[1].Trim().Split(", ").Select(long.Parse).Concat(zeroArray).ToArray();
                Hailstones.Add(new Hailstone(new Vector<long>(coords), new Vector<long>(velocity)));
            }
        }

        static string input = 
@"262130794315133, 305267994111063, 163273807102793 @ 57, -252, 150
290550702673836, 186986670515285, 231769402282435 @ -74, 19, -219
275698513286341, 162656001312879, 183065006152383 @ -59, -24, -225
300978671520025, 310644717172257, 264594178261059 @ -12, -15, 13
255863566365481, 175389174276099, 191768173516493 @ 111, 36, -70
448611932895831, 416105494312867, 530277693479337 @ -278, -232, -476
321245794812141, 351009956410617, 308560443071587 @ -14, 39, 33
260635600256893, 260747163777350, 216665142277310 @ 73, -322, -130
255145296218621, 187412180345097, 228199618141667 @ 69, 145, 13
230194982752265, 194323808433321, 261417581945831 @ 152, 91, -142
292048304292386, 197291170834422, 202943740926287 @ -72, -10, -55
311130681683078, 306087457111806, 249396912802046 @ -6, 76, 88
358639652963990, 394739736663027, 359795847374705 @ -270, -615, -525
297229229558482, 522171337518307, 426770062212680 @ -30, -655, -466
294834208751831, 321617515744254, 386879228152130 @ 11, 56, -64
266515787823596, 378913088003187, 283788686103107 @ 42, -27, 37
253564419687295, 253989458064767, 121056322127979 @ 150, -650, 409
314979912243887, 395540618713041, 248874251248658 @ -17, -57, 74
311505400730033, 259696521745221, 198440344941707 @ -90, -102, 47
415151793286817, 244561703933778, 365788651924237 @ -115, 144, -33
360707651284924, 529063939149571, 260034369166300 @ -108, -367, 18
293112415768602, 215942318820166, 195865614624459 @ -32, 38, 61
276985563220526, 415192274624267, 310163091739654 @ 26, -175, -56
337855258250936, 317163569817867, 330559462936061 @ -74, -35, -101
234788458620675, 370907912387149, 262262841563777 @ 88, -79, 32
259483935349211, 442042685387769, 337046080143827 @ 51, -129, -43
234477844664477, 141803820106285, 408255494535747 @ 137, 248, -563
286953422958549, 349980214383283, 253535024212593 @ 13, -46, 46
288459711073005, 76121970967305, 114880375890659 @ 18, 324, 233
373928783591119, 397930231995023, 268319757027179 @ -182, -286, -55
346657310924317, 314049384684153, 397819026112163 @ -38, 81, -52
323308101707874, 224678126324224, 174511679867046 @ -62, 98, 148
263017161177728, 197905391546181, 253039195405385 @ 52, 107, -69
331699490975885, 425647191528329, 310132560849731 @ -28, -52, 23
329144010696083, 154543668426651, 284138920129493 @ -110, 218, -136
42086128312493, 149739688734057, 163398867975947 @ 259, 245, 182
320793558333081, 385998654679497, 304073501483947 @ -11, 15, 45
292555602225809, 76299698455005, 234376740055559 @ -56, 498, -136
277061229442591, 170433390623257, 98447783685452 @ 31, 223, 248
233285123334485, 360925668194061, 341017914152891 @ 77, 21, -7
272323574705558, 182746493452253, 205930487329143 @ 14, 19, -119
273639662675533, 244504640510813, 231820236849925 @ 25, -13, -17
303638664941189, 162724508183685, 168625769826563 @ -15, 219, 165
205334005283501, 171972117823143, 236292337347689 @ 161, 192, 26
246617162136253, 296960477921634, 212735340817807 @ 73, 12, 97
260306154721631, 198700968719541, 192066878054801 @ 76, -39, -22
260778014462018, 371835967112580, 346451734407386 @ 48, 8, -14
305521978060639, 302870740283105, 240016390345043 @ -128, -469, -211
234805030786946, 278948963717247, 275554182933462 @ 104, -15, -50
175016960742041, 335922640620225, 249111586529303 @ 166, -11, 60
254595198166793, 317134897796313, 283294013962331 @ 114, -761, -577
306875815045877, 484026988453461, 212102925660578 @ -31, -373, 80
296888770066151, 222634409919369, 179952286484855 @ -64, -41, 83
338642608643806, 329758654804240, 370954681079301 @ -44, 28, -69
285035808691781, 217199988552717, 179144997950807 @ -41, -106, 52
283946133250432, 296437886741900, 207394653636810 @ 24, 94, 136
285316391399807, 163282745190711, 158355081333227 @ -62, 118, 137
349569505838657, 529115545331321, 189761373535392 @ -67, -257, 142
251092123158605, 122175575227353, 257022719268419 @ 58, 273, 83
303978114677513, 419088507527122, 316724679122389 @ 5, -20, 31
259197467512180, 310297887313481, 43603579703605 @ 54, -17, 363
244000487057331, 224853534463865, 196629937331745 @ 76, 127, 124
303061672985111, 293270310160947, 235908526098002 @ -17, 5, 54
266874859572965, 167594872930317, 168488307110099 @ 47, -90, -55
292977243379667, 444781293003507, 288764784937100 @ 15, -55, 54
248972142889361, 153061171319967, 98292760942967 @ 173, 157, 553
188853770969678, 58515325915323, 335056351381439 @ 140, 357, -39
248947645469555, 259119274316106, 32277306089606 @ 59, 138, 310
201964080493466, 84468930595554, 373749557160606 @ 116, 318, -62
227713828114775, 342745669543497, 294302733727043 @ 80, 55, 53
244718305784914, 378674142181104, 379289225055922 @ 64, 12, -38
358451376825251, 127260954432069, 511231040678028 @ -49, 267, -162
332220092307767, 346643567939741, 329044711130021 @ -46, -23, -46
400404820519162, 225559263245478, 351267102337288 @ -186, 108, -150
307240915224141, 367096670090653, 169032200686015 @ -59, -316, 144
295189288731776, 206901876412209, 185774813746814 @ -114, -121, -14
325973977257823, 253431964019310, 213470324198173 @ -161, -138, -29
238617477244216, 410189892454597, 308328327826882 @ 72, -37, 24
343375092612706, 215786559306775, 217604741815551 @ -164, 49, 9
275393271010493, 304484965016835, 162950833235567 @ 25, -74, 167
355725997371389, 255299425623273, 126127945931843 @ -97, 74, 231
247768592037813, 342908035342257, 273767927835583 @ 67, -15, 30
315404244428186, 312499199762402, 269902185938307 @ -22, 28, 37
285783023673389, 331478376612969, 431767854900723 @ -22, -402, -774
298666311274225, 333337180662087, 303043132925819 @ -5, -30, -31
273659148934007, 147511757086282, 158197366461215 @ -25, 170, 89
292084340638971, 157832857699163, 140257529815665 @ -69, 173, 231
292707119199866, 260344910494347, 219658577523857 @ -60, -227, -89
127222802025273, 41222261479629, 333355798492591 @ 208, 372, -24
219492606131741, 345080228095097, 349139819606307 @ 95, 23, -30
418354968789841, 390748674625422, 376224157264507 @ -123, -18, -51
270981616420556, 233435101185087, 179890241999732 @ 28, -83, 82
227932946520700, 232435056611118, 385639478206094 @ 144, 12, -424
353230018283885, 180686999303997, 336140332022315 @ -63, 205, -31
305652996804134, 286453493146902, 264981285923474 @ -28, -10, -14
286106337728091, 174242436075437, 298709827360427 @ -44, 97, -487
273738565771373, 300866810112441, 268993785517715 @ 32, 27, 26
234567510192191, 258789033954291, 447668032787426 @ 91, 73, -260
252644048077631, 154806280520049, 168658485149857 @ 236, 51, -78
309301413952251, 103389901433699, 338831571174869 @ -44, 328, -190
312003569811791, 295994692938447, 291024528879407 @ -85, -187, -207
230580989623922, 222837917179263, 103715983594292 @ 220, -151, 412
234629600530601, 52805503022583, 146856436775318 @ 261, 837, 204
275412161287677, 296323398483849, 389309725205987 @ 33, 99, -43
326552386579147, 288497805144983, 203339272577943 @ -38, 56, 123
218892698969971, 123715301099657, 120439664113087 @ 214, 311, 295
266547705892363, 242398012855125, 456057288735013 @ 42, 129, -180
256357426966481, 193159949271690, 185691047123513 @ 101, -38, -9
300632416765387, 215886312801611, 196941584463381 @ -56, 32, 54
300814185860415, 281852740758988, 241324765535693 @ -17, 8, 35
300635266484551, 489374203723587, 388790839552587 @ -38, -576, -375
229743235491293, 82581092792187, 177290106139757 @ 137, 400, 122
271109848230041, 304798639680947, 222792895981907 @ 35, -10, 77
543871991141381, 463523134711377, 354820393128827 @ -261, -99, -29
398517274082141, 446208187491297, 275146852212507 @ -127, -138, 34
345850580377127, 242467526770517, 247788626678471 @ -58, 125, 71
286399940271707, 292416586142916, 321407870319074 @ 15, 46, -40
261133634593208, 166408826923140, 332764960982786 @ 50, 216, -70
356593483519241, 441971112394107, 388814644396397 @ -69, -118, -100
284320628252097, 437623532373829, 327427096220147 @ 24, -44, 17
266615400478450, 168099007551998, 154168272645759 @ 52, -166, 105
265333520939124, 201548346129217, 147898020991907 @ 58, -265, 197
280139185097237, 320017097241871, 1350929472589 @ 5, -254, 610
280340374227966, 231789711346363, 281802804785333 @ 16, 77, -61
142313351062015, 222574181774243, 127554673545831 @ 250, 116, 231
297125924913300, 234236879581285, 243737951199097 @ -18, 67, 7
259386160034220, 238907921487053, 266643488340096 @ 58, 43, -56
312765948828441, 173910432449101, 324507305505238 @ -59, 179, -192
359055761240277, 345452210429497, 307583996247627 @ -48, 55, 42
275765745072013, 138136629216489, 169111149391955 @ -98, 276, -154
269363010687205, 326210021067781, 143902208578579 @ 37, -119, 205
260893454623283, 173807718933279, 194562022594241 @ 79, 53, -77
426645144795101, 359622513149297, 348808071843227 @ -223, -110, -136
234081985272869, 231537323623905, 322031441774075 @ 110, 64, -164
235756796813009, 83001310682808, 131003925514433 @ 157, 462, 259
265450831349227, 410275438825628, 310611237153597 @ 43, -28, 27
239707310377766, 225329337757318, 140522609231167 @ 116, 22, 217
277151859522653, 198757724548473, 284912591581475 @ 10, 67, -237
265340091807427, 95868018272746, 42597361989979 @ 43, 298, 301
280710474743896, 322099307798212, 253740612735757 @ 12, -142, -33
311742164640651, 291451783752287, 260659403365957 @ -20, 46, 42
243652796324075, 245470304255376, 250758168535496 @ 115, -76, -124
308479096657001, 246758992538787, 134193356933087 @ -67, -27, 233
317857816248325, 248644830626922, 223151581799662 @ -63, 30, 42
254132894624004, 235541846028925, 301283637288091 @ 90, -101, -371
321459307356359, 301259080301708, 241918897418924 @ -85, -122, -22
348989189188161, 407599211926877, 352406716742867 @ -50, -44, -31
150429051498666, 275987100627527, 14123176380642 @ 166, 109, 340
260950420699336, 380932580071807, 321652281674112 @ 64, -611, -426
278926523728541, 173627591718777, 173911214299907 @ -59, -41, -28
260651555885941, 323357022236737, 289218218908237 @ 61, -293, -224
248146745047129, 222243546923197, 149886127140191 @ 70, 130, 194
212439724513880, 271473558304983, 318228412798631 @ 130, 41, -79
398030382080932, 167961778564650, 164339347312486 @ -110, 222, 178
439592971331597, 248577553338809, 197778222866803 @ -142, 139, 144
325703381454683, 391181512422573, 359758079476949 @ -16, 9, -10
256735876714909, 341361118291705, 257941790800803 @ 58, -68, 21
276637888512495, 157951358808605, 175264042159381 @ -41, 91, -45
263926170374621, 142307391396567, 134464684640657 @ 73, 228, 322
314040985818041, 198787884224072, 201315974078157 @ -139, 26, -9
380518795560477, 486822982888563, 458186658250169 @ -87, -138, -154
348155161796373, 248008818611881, 266103145835153 @ -203, -73, -160
251567357891462, 264572552181246, 129496250239793 @ 132, -466, 303
302631163602710, 346052523144775, 247509170442244 @ -16, -79, 36
283353418392577, 155915462719082, 196558106811147 @ -51, 160, -83
261756677486856, 142886883361467, 203402108825962 @ 70, 237, -90
250244594199191, 277348187868717, 222249951356627 @ 76, -27, 45
246007595614981, 263813279886584, 66739487302779 @ 81, 22, 349
320864077417786, 468181297159039, 257814092662974 @ -14, -83, 84
169242763654287, 364878403707153, 370239069885440 @ 135, 39, -16
376031102929911, 248071789194067, 284758440660573 @ -144, 70, -36
299008238538759, 59367275185675, 434547588342823 @ 7, 341, -111
312886333405315, 209353044946179, 336644579948287 @ -41, 129, -143
313142914187509, 324536638824347, 298695511791331 @ -27, -20, -27
206906764114733, 208451963548830, 254730656524670 @ 217, 54, -114
264120582458761, 135187912116383, 95985668643427 @ 51, 268, 357
259366028224733, 180673752337161, 152897443651787 @ 129, -199, 142
276150727560456, 240041109660087, 157801814281993 @ -14, -375, 135
320755852656943, 154073578705247, 472936202197695 @ -50, 230, -357
251697747369581, 234045096536577, 195958697534987 @ 189, -639, -257
273867305298473, 175055788332669, 139228419901331 @ -46, -222, 313
263220501240434, 213255084841149, 193865438745821 @ 52, 59, 75
216722269303013, 272918719849089, 394781842482356 @ 129, 23, -232
183515972164350, 275920493771884, 201949527774245 @ 162, 58, 119
383228879810589, 181598957483385, 208173973475747 @ -132, 192, 107
293815666438966, 276948712928507, 252257510900602 @ -14, -31, -20
386078415778781, 398937650617401, 178190383185251 @ -104, -62, 160
380995039761026, 555025193662507, 338304939570017 @ -70, -151, 11
490250636800352, 379945622654196, 381081894739036 @ -262, -72, -120
350706732724509, 500073871593593, 373135955944355 @ -191, -749, -430
301566333801569, 313764999047977, 321586616955559 @ -10, -5, -62
166844687178572, 355854864497112, 277033739569619 @ 178, -40, 21
346454435824821, 417514996865157, 298945678395327 @ -81, -174, -36
296543817322666, 362831363364397, 271282292438607 @ 6, -13, 49
266457709131590, 176090488423716, 169416253552745 @ 48, -28, 31
291258619065335, 136750955790831, 36394952929535 @ 14, 258, 323
171753734178107, 210375055299081, 360805814655059 @ 138, 183, -19
244865387046746, 260777505915732, 196383327574592 @ 112, -128, 44
409982369539821, 372207502826757, 511991509437363 @ -299, -300, -671
299053719793761, 280585666431207, 332590622183137 @ -93, -342, -584
279736319780273, 194045324985180, 205630359057323 @ 5, 96, 29
308472882877122, 348137519884618, 129938311983692 @ -62, -268, 242
299766072988197, 93697281599968, 245679515238226 @ -15, 334, 28
390181142882474, 365160127229175, 252966076965815 @ -132, -63, 49
264625180790501, 241467948198687, 178379672043602 @ 65, -639, -70
322000155193759, 348416624287124, 181568459222146 @ -45, -74, 144
410849542104266, 333327182199792, 304426498399607 @ -124, 32, 17
294128058785401, 166168745049557, 174059560593287 @ -137, 77, 23
420371139643274, 441924572596128, 221257078946642 @ -178, -178, 92
241374017484303, 229048519364375, 246558065508239 @ 139, -83, -175
301118707335806, 224020986943512, 265804441924682 @ -32, 72, -58
399619956717285, 516972062452821, 228957446556363 @ -207, -453, 45
322640486889299, 337368788208621, 280944580975778 @ -45, -53, -10
364969873356235, 234167702274497, 350688511665679 @ -165, 55, -231
231245687770513, 97922104226249, 287726653998055 @ 142, 371, -194
246281033598919, 191970225523303, 160043223112657 @ 258, -286, 72
360245363936547, 422220962796131, 390012854630035 @ -48, -16, -35
323907161030577, 220609808130387, 209552923081042 @ -147, -15, -8
207283122246206, 351869611979157, 200143222934807 @ 110, 11, 137
264687190459061, 107382402237832, 122234678286057 @ 65, 542, 427
288582191326061, 489487723695777, 185911968256797 @ 17, -141, 154
287623077192251, 219841858864722, 317707498134182 @ 7, 120, -88
259947855027717, 162673600672049, 171572255546811 @ 114, 26, -36
247440519992373, 386309087712941, 47736596267603 @ 115, -662, 569
446956532382989, 465736556079633, 504890737432139 @ -205, -192, -292
303723250807823, 136790096764317, 194195804444633 @ -98, 265, 20
292343880908003, 280631949776886, 524501111744411 @ 15, 108, -195
254824074019239, 212432322103429, 196175439139397 @ 154, -403, -238
286387481600822, 214974138208827, 174737813367098 @ -32, -35, 94
313254774789791, 295430669796147, 403044587911337 @ -9, 85, -81
437655358369325, 240248063997273, 199270499818691 @ -138, 149, 143
282790546429398, 103722840694329, 174474384831504 @ -42, 447, 54
232950077865831, 117128844056337, 223296917475587 @ 282, 411, -331
313423041778891, 430491954470269, 387399294943327 @ -9, -61, -63
459759241089677, 407401959354713, 543635554017203 @ -328, -259, -562
186058282220357, 294560093773305, 309765731363099 @ 150, 47, -20
246784142466821, 24209572806792, 403730812690232 @ 113, 658, -698
354294398943965, 402953334276665, 208610544109219 @ -85, -127, 109
269665915720269, 198364078762727, 172056539009051 @ 33, 50, 113
249738141860063, 198780822729465, 9665524652369 @ 60, 191, 346
215278981413761, 324600894982137, 176834756029007 @ 131, -65, 147
214539297127141, 60167079714597, 221325893213133 @ 141, 405, 58
404970496788829, 549186554603937, 303715891693939 @ -95, -150, 43
241529639474541, 227345738938197, 236334805050907 @ 145, -99, -159
339404627137859, 255726487879449, 286028848839425 @ -68, 79, -12
335544740738051, 310931401960959, 258420799767239 @ -54, 16, 43
226816368019301, 237398570526027, 240484302578405 @ 161, -35, -77
304060768791396, 328888857368021, 303354198890620 @ -14, -29, -36
457384732426809, 372882822391681, 350699238261915 @ -182, -19, -41
313498344076185, 212092464758764, 190544445966950 @ -131, -16, 38
267473683747754, 157755686903694, 147029386854203 @ 40, 86, 205
269703436889789, 245215332263169, 77894194490045 @ 25, -469, 676
282646653257706, 206644605364223, 205134367320925 @ -14, 13, -9
361955006588404, 389582821553344, 465260402830438 @ -66, -28, -162
363637479303008, 294380833141839, 244170395814374 @ -82, 57, 74
259799829626949, 280309034892848, 346486485898697 @ 49, 106, -13
254692917163001, 290921073889197, 194179052934227 @ 64, -20, 113
361400313111013, 337156303011719, 125772089549443 @ -95, -31, 229
173572869500219, 65323518187776, 361309338265973 @ 179, 364, -117
303852383166080, 232717519842141, 236424533212559 @ -20, 99, 49
233021281924366, 224953130566442, 287146610628762 @ 96, 118, -26
293753003591921, 255172057813977, 255152062541387 @ -21, -17, -55
380923310061515, 197328096947073, 362058126819113 @ -108, 179, -84
284695332187169, 248201109125195, 229735207384317 @ 5, 29, 27
200094709366157, 222807352034469, 150986967706343 @ 237, 12, 188
310802308328111, 265262304456887, 279880294221062 @ -37, 29, -40
372777841360367, 497283908123181, 458884351502141 @ -240, -699, -632
266322430790147, 179143243611935, 166202736889935 @ 50, -88, 39
278211019222472, 147027244347135, 148679708540875 @ -70, 177, 189
288812980953975, 188567215470321, 172478733354719 @ -137, -153, -7
241397675961401, 245439457197852, 225078989090462 @ 77, 108, 90
204581885908621, 75725669050321, 254629492564171 @ 111, 326, 78
263062172415656, 170386901611869, 125427153541952 @ 86, -69, 432
352672330726091, 130306353278505, 243442114386923 @ -109, 271, 29
265571119161479, 167411894892276, 171120164288654 @ 59, -26, -36
324567283213181, 250985349260217, 144367022304707 @ -40, 97, 202
265666811447903, 372071936216103, 303008784639511 @ 44, -156, -77
204252173106731, 362899818315483, 233286104615297 @ 126, -46, 82
301681866245177, 315609163651578, 258767559000731 @ -11, -12, 29
314479995922841, 358961184865815, 307146354066881 @ -9, 22, 28
172917870915559, 183427806389392, 238343253134644 @ 159, 200, 84
256878222404813, 183101132726897, 199021571111081 @ 65, 155, 80
419277343679842, 268672727206214, 446061173706063 @ -120, 118, -119
257606232211325, 395576187811617, 468111017325557 @ 57, -165, -328
293924054794016, 417782936622972, 285611096691407 @ 14, -28, 57
407103851018769, 407663384464069, 512544750097137 @ -233, -271, -518
259467084653501, 175887120421977, 157864569821267 @ 125, -133, 91
241115395532897, 169405458113433, 228209630510294 @ 125, 159, -60
257673685340558, 3366981335306, 404541157476642 @ 68, 634, -518
294896284432779, 423116933303353, 181028762968329 @ -18, -353, 126
374687015146295, 432447887243387, 252175338447285 @ -100, -130, 60
293454485170041, 289158835902427, 267387384549817 @ -9, -32, -32
272944568455766, 192522287788839, 180482090607221 @ 16, 17, 51
323117179880881, 240266363514403, 258895334743493 @ -69, 56, -22
325435609468771, 337675086347368, 247447458114587 @ -29, 16, 77
243128014230038, 261834169702366, 210044406249607 @ 80, 58, 97
327172301750711, 221996682213027, 330167574577187 @ -22, 168, 5
330765545428461, 273339222247178, 330966794229026 @ -39, 86, -34
442928938882860, 423875533118508, 246476644586792 @ -146, -48, 92
272272704715837, 382945452421261, 343588580374335 @ 33, -142, -122"; //paste it manually from the page
    }
}
