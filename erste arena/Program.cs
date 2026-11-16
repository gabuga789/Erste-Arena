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

            // --- Gegner ---
            string gegnerName = "Troll";
            int gegnerLebenspunkte = 600;
            int gegnerRuestung = 8;

            // --- Waffe ---
            Console.Write("Waffe (Schwert/Axt/Bogen/Dolch/Hellebarde/Streitkolben/Speer/Großschwert): ");
            string waffenName = Console.ReadLine();
            int waffenBonus;
            bool istFernkampf = false;

            switch (waffenName)
            {
                case "Schwert":
                case "schwert":
                    waffenBonus = 6;
                    break;
                case "Axt":
                case "axt":
                    waffenBonus = 10;
                    break;
                case "Bogen":
                case "bogen":
                    waffenBonus = 1;
                    istFernkampf = true;
                    break;
                case "Dolch":
                case "dolch":
                    waffenBonus = 2;
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
                    waffenBonus = 5;
                    istFernkampf = true;
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

            int grundschaden = 10;

            // --- A6.6: Schadensberechnung über Methode ---
            int schaden = BerechneSchaden(grundschaden, waffenBonus, staerke, gegnerRuestung, istFernkampf);

            // --- Vorsprung ---
            int vorsprung = lebenspunkte - gegnerLebenspunkte;

            // --- A6.3 & A6.9: Ausgabe Heldenprofil ---
            Ueberschrift("HELDENPROFIL");
            ZeigeHeldenprofil(heldenName, lebenspunkte, staerke, gold, kritischeChance, hatSchild);

            // --- A6.3: Ausgabe Gegner ---
            Ueberschrift("GEGNER");
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

            // --- A6.3: Händler ---
            Ueberschrift("HÄNDLER");
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

            // --- A6.3: Kampfbilanz ---
            Ueberschrift("KAMPFBILANZ");
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

                // --- A6.8: Rang über Methode bestimmen ---
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

            // --- Zufallsgeneration ---
            Random zufall = new Random();

            // Anzahl der Angriffe für den Kampf festlegen
            int angriffe = 1;
            if (istFernkampf)
            {
                angriffe = 2;
            }

            // --- Das Spielmenü ---
            int wahl;
            do
            {
                // --- A6.1: Menü über Methode anzeigen ---
                ZeigeMenue();
                Console.Write("Deine Wahl: ");
                wahl = int.Parse(Console.ReadLine());

                if (wahl == 1)
                {
                    // --- A6.11: Kampf über Methode ausführen ---
                    lebenspunkte = Kampf(heldenName, lebenspunkte, schaden, angriffe,
                                         gegnerName, gegnerLebenspunkte, zufall);
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

        // ==========================================
        // AB HIER DIE METHODEN AUS MODUL 6
        // ==========================================

        // A6.1: Menü anzeigen
        static void ZeigeMenue()
        {
            Console.WriteLine();
            Console.WriteLine("=== WAS WILLST DU TUN? ===");
            Console.WriteLine("1 = Kämpfen");
            Console.WriteLine("2 = Heiltrank kaufen");
            Console.WriteLine("3 = Beenden");
        }

        // A6.3: Abschnittsüberschriften
        static void Ueberschrift(string titel)
        {
            Console.WriteLine();
            Console.WriteLine("=== " + titel + " ===");
        }

        // A6.6: Schadensberechnung
        static int BerechneSchaden(int grundschaden, int waffenBonus, int staerke,
                                   int gegnerRuestung, bool istFernkampf)
        {
            int abzug = gegnerRuestung;
            if (istFernkampf)
            {
                abzug = gegnerRuestung / 2;
            }
            return (grundschaden + waffenBonus) * staerke - abzug;
        }

        // A6.7: Prüfen, ob jemand besiegt ist
        static bool IstBesiegt(int lebenspunkte)
        {
            return lebenspunkte <= 0;
        }

        // A6.8: Rang anhand der Siegquote berechnen
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

