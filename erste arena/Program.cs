using System;
namespace Arena
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // --- Held ---
            Console.Write("Wie heißt dein Held? ");
            string heldenName = Console.ReadLine();

            Console.Write("Lebenspunkte: ");
            int lebenspunkte = int.Parse(Console.ReadLine());

            Console.Write("Stärke: ");
            int staerke = int.Parse(Console.ReadLine());

            Console.Write("Gold: ");
            int gold = int.Parse(Console.ReadLine());

            Console.Write("Kritische Chance (z. B. 0,15): ");
            double kritischeChance = double.Parse(Console.ReadLine());

            if (kritischeChance < 0 || kritischeChance > 1)
            {
                Console.WriteLine("Die Chance muss zwischen 0 und 1 liegen - sie wird auf 0 gesetzt.");
                kritischeChance = 0;
            }

            bool hatSchild = true;

            // --- Gegner (Anpassung Troll statt Goblin) ---
            string gegnerName = "Troll";
            int gegnerLebenspunkte = 1200;
            int gegnerRuestung = 8;

            // --- Waffe ---
            Console.Write("Waffe (Schwert/Axt/Bogen/Dolch/Hellebarde/Streitkolben/Speer/Großschwert): ");
            string waffenName = Console.ReadLine();
            int waffenBonus;

            switch (waffenName)
            {
                case "Schwert":
                case "schwert":
                    waffenBonus = 8;
                    break;
                case "Axt":
                case "axt":
                    waffenBonus = 10;
                    break;
                case "Bogen":           //Fernkampf Waffe
                case "bogen":
                    waffenBonus = 3;
                    break;
                case "Dolch":
                case "dolch":
                    waffenBonus = 4;
                    break;
                case "Hellebarde":
                case "hellebarde":
                    waffenBonus = 12;
                    break;
                case "Streitkolben":
                case "streitkolben":
                    waffenBonus = 18;
                    break;
                case "Speer":
                case "speer":
                    waffenBonus = 7;    //Fernkampf Waffe
                    break;
                case "Großschwert":
                case "großschwert":
                    waffenBonus = 15;
                    break;
                default:
                    Console.WriteLine("Unbekannte Waffe - du kämpfst mit bloßen Fäusten.");
                    waffenName = "bloßen Fäusten";
                    waffenBonus = 0;
                    break;
            }
            ;

            // --- Schadensberechnung ---
            bool istFernkampf = waffenName == "Bogen" || waffenName == "Speer";
            int grundschaden = 10;
            int schaden = BerechneSchaden(grundschaden, waffenBonus, staerke,
                            gegnerRuestung, istFernkampf);


            // --- Vorsprung ---
            int vorsprung = lebenspunkte - gegnerLebenspunkte;

            // --- Ausgabe/Helden Profil ---
            AusgabenUberschrift("HELD");
            ZeigeHeldenprofil(heldenName, lebenspunkte, staerke,
                               gold, kritischeChance, hatSchild);

            Console.WriteLine();
            Console.WriteLine("=== GEGNER ===");
            Console.WriteLine($"Name:         {gegnerName}");
            Console.WriteLine($"Lebenspunkte: {gegnerLebenspunkte}");
            Console.WriteLine($"Rüstung:      {gegnerRuestung}");

            Console.WriteLine();
            Console.WriteLine($"{heldenName} greift mit {waffenName} an und verursacht {schaden} Schaden.");

            if (schaden >= gegnerLebenspunkte)
            {
                Console.WriteLine($"{gegnerName} fällt mit einem Schlag!");
            }
            else
            {
                Console.WriteLine($"{gegnerName} übersteht den ersten Angriff.");
            }

            if (vorsprung > 0)
            {
                Console.WriteLine($"{heldenName} ist um {vorsprung} LP im Vorteil.");
            }
            else if (vorsprung < 0)
            {
                Console.WriteLine($"{gegnerName} ist um {-vorsprung} LP im Vorteil.");
            }
            else
            {
                Console.WriteLine("Gleichstand.");
            }

            // --- Händler ---
            AusgabenUberschrift("HÄNDLER");
            Console.Write("Was kostet ein Heiltrank? ");
            int trankPreis = int.Parse(Console.ReadLine());

            if (trankPreis <= 0)
            {
                Console.WriteLine("Der Preis muss größer als 0 sein.");
            }
            else
            {
                int traenke = gold / trankPreis;
                int restGold = gold % trankPreis;
                Console.WriteLine($"Du kannst {traenke} Tränke kaufen, {restGold} Gold bleiben übrig.");
            }

            // --- Kampfbilanz ---
            AusgabenUberschrift("KAMPFBILANZ");
            Console.Write("Bestrittene Kämpfe: ");
            int kaempfe = int.Parse(Console.ReadLine());

            Console.Write("Davon gewonnen: ");
            int siege = int.Parse(Console.ReadLine());

            if (kaempfe <= 0 || siege < 0 || siege > kaempfe)
            {
                Console.WriteLine("Unsinnige Eingabe - die Bilanz kann nicht berechnet werden.");
            }
            else
            {
                int niederlagen = kaempfe - siege;
                double siegquote = (double)siege / kaempfe;

                Console.WriteLine($"Siege:       {siege}");
                Console.WriteLine($"Niederlagen: {niederlagen}");
                Console.WriteLine($"Siegquote:   {siegquote:F2}");


                char rang = BerechneRang(siegquote);

                Console.WriteLine($"Rang:        {rang}");

                switch (rang)
                {
                    case 'A':
                        Console.WriteLine("Titel:       Legende der Arena");
                        break;
                    case 'B':
                        Console.WriteLine("Titel:       Erfahrener Kämpfer");
                        break;
                    case 'C':
                        Console.WriteLine("Titel:       Solider Anfänger");
                        break;
                    default:
                        Console.WriteLine("Titel:       Noch viel zu lernen");
                        break;
                }

                bool bossFrei = kaempfe >= 5 && siegquote >= 0.5 && lebenspunkte >= 80;

                if (bossFrei)
                {
                    Console.WriteLine("Die Bossarena steht dir offen.");
                }
                else
                {
                    Console.WriteLine("Für den Bosskampf fehlt noch etwas.");
                }
            }

            // --- Zufalls generation ---
            Random zufall = new Random();

            // --- Das spielmenü  ---
            int wahl;
            do
            {
                ZeigeMenue();
                wahl = int.Parse(Console.ReadLine());

                if (wahl == 1)
                {
                    // --- Kampf ---
                    int gegnerLP = gegnerLebenspunkte;
                    int runde = 1;

                    while (!IstBesiegt(lebenspunkte) && gegnerLP > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"-- Runde {runde} --");

                        // Anzahl der Angriffe bestimmen (A5.6: Fernkampf schießt zweimal)
                        int angriffe = 1;
                        if (istFernkampf)
                        {
                            angriffe = 2;
                        }

                        // Angriffe des Helden ausführen
                        for (int i = 0; i < angriffe; i++)
                        {
                            if (gegnerLP > 0) // Nur angreifen, wenn der Troll noch lebt
                            {
                                // A5.7: Schaden schwankt um bis zu 5 nach oben oder unten
                                int rundenSchaden = schaden + zufall.Next(-5, 6);
                                gegnerLP = gegnerLP - rundenSchaden;

                                // Negative Lebenspunkte optisch abfangen
                                if (gegnerLP < 0) gegnerLP = 0;

                                Console.WriteLine($"{heldenName} trifft für {rundenSchaden}. {gegnerName}: {gegnerLP} LP");
                            }
                        }

                        // Gegenschlag des Gegners (nur wenn er die Angriffe überlebt hat)
                        if (gegnerLP > 0)
                        {
                            // A5.7 Zuffal
                            int gegnerSchaden = zufall.Next(10, 31);
                            lebenspunkte = lebenspunkte - gegnerSchaden;

                            if (lebenspunkte < 0) lebenspunkte = 0;

                            Console.WriteLine($"{gegnerName} trifft für {gegnerSchaden}. {heldenName}: {lebenspunkte} LP");
                        }

                    }
                }
                else if (wahl == 2)
                {
                    if (trankPreis > 0 && gold >= trankPreis)
                    {
                        gold = gold - trankPreis;
                        lebenspunkte = lebenspunkte + 30;
                        Console.WriteLine($"Getrunken. Lebenspunkte: {lebenspunkte}, Gold: {gold}");
                    }
                    else
                    {
                        Console.WriteLine("Dafür reicht dein Gold nicht.");
                    }
                }
            }
            while (wahl != 3);
        }
        static void ZeigeMenue()
        {
            AusgabenUberschrift("WAS WILLST DU TUN?");
            Console.WriteLine("1 = Kämpfen");
            Console.WriteLine("2 = Heiltrank kaufen");
            Console.WriteLine("3 = Beenden");
            Console.Write("Deine Wahl: ");
        }
        static string AusgabenUberschrift(string uberschrift)
        {
            Console.WriteLine();
            Console.WriteLine($"=== {uberschrift} ===");
            return uberschrift;
        }
        static int BerechneSchaden(int grundschaden, int waffenBonus, int staerke, int gegnerRuestung, bool istFernkampf)
        {

            int ruestungsAbzug = gegnerRuestung;

            if (istFernkampf)
            {
                ruestungsAbzug = gegnerRuestung / 2;
            }
            return (grundschaden + waffenBonus) * staerke - gegnerRuestung;
        }
        static bool IstBesiegt(int lebenspunkte)
        {
            return lebenspunkte <= 0;
        }
        static char BerechneRang(double siegquote)
        {
            if (siegquote >= 0.75)
            {
                return 'A';
            }
            else if (siegquote >= 0.50)
            {
                return 'B';
            }
            else if (siegquote >= 0.25)
            {
                return 'C';
            }
            else
            {
                return 'D';
            }
        }
        static void ZeigeHeldenprofil(string heldenName, int lebenspunkte, int staerke,
                              int gold, double kritischeChance, bool hatSchild)
        {
            Console.WriteLine($"Name:         {heldenName}");
            Console.WriteLine($"Lebenspunkte: {lebenspunkte}");
            Console.WriteLine($"Stärke:       {staerke}");
            Console.WriteLine($"Gold:         {gold}");
            Console.WriteLine($"Krit-Chance:  {kritischeChance}");
            Console.WriteLine($"Schild:       {hatSchild}");

            if (lebenspunkte < 20)
            {
                Console.WriteLine("Kritisch! Nimm einen Heiltrank.");
            }
            else if (lebenspunkte < 60)
            {
                Console.WriteLine("Angeschlagen.");
            }
            else
            {
                Console.WriteLine("Alles im grünen Bereich.");
            }
        }
    }
}
