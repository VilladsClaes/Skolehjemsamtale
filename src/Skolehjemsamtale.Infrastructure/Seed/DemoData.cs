namespace Skolehjemsamtale.Infrastructure.Seed;

/// <summary>
/// Demodata der viser værktøjets formål: opfølgning på konkrete observationer,
/// inddragelse af hjem og elev, og indsamling af viden fra hjemmet.
/// </summary>
public static class DemoData
{
    public static SeedRoot Bygg()
    {
        var iDag = DateOnly.FromDateTime(DateTime.UtcNow);
        string D(int minusDage) => iDag.AddDays(-minusDage).ToString("yyyy-MM-dd");

        return new SeedRoot
        {
            Version = 1,
            Elever = new List<SeedElev>
            {
                new()
                {
                    Navn = "Emma Sørensen",
                    Klasse = "7B",
                    Foedselsdato = "2012-04-12",
                    Observationer = new List<SeedObservation>
                    {
                        new()
                        {
                            Dato = D(21), Kategori = "Deltagelse", Valens = "Opmærksomhed",
                            Beskrivelse = "Kom 12 minutter for sent til matematik og blev stående uden for lokalet.",
                            Kontekst = "Matematik, 1. lektion", ObserveretAf = "Lærer (undertegnede)"
                        },
                        new()
                        {
                            Dato = D(14), Kategori = "Fravær", Valens = "Opmærksomhed",
                            Beskrivelse = "Gik hjem efter 3. lektion efter at have klaget over ondt i maven.",
                            Kontekst = "Efter idræt", ObserveretAf = "Teamet", HvadHjalp = "En kort pause med en kendt voksen hjalp en enkelt gang."
                        },
                        new()
                        {
                            Dato = D(9), Kategori = "Faglig", Valens = "Styrke",
                            Beskrivelse = "Viste stort overskud i gruppearbejdet og hjalp en klassekammerat med opgaven.",
                            Kontekst = "Naturfag, gruppearbejde", ObserveretAf = "Lærer (undertegnede)", DeltMedHjem = true
                        }
                    },
                    Samtaler = new List<SeedSamtale>
                    {
                        new()
                        {
                            Type = "SkoleHjemSamtale", Dato = D(3),
                            Formaal = "Forstå sammen, hvad der gør morgener og enkelte lektioner svære for Emma, og aftale et lille, testbart første skridt.",
                            SkabelonKode = "skolevaegring-opfoelgning", Sted = "Grupperum 2", Status = "Afholdt",
                            Deltagere = new List<SeedDeltager>
                            {
                                new() { Navn = "Emma", Rolle = "Elev" },
                                new() { Navn = "Mor (Anne)", Rolle = "Foraelder" },
                                new() { Navn = "Kontaktlærer", Rolle = "Laerer" }
                            },
                            Noter = new List<SeedNote>
                            {
                                new() { Fase = "Aabning", Perspektiv = "Skole", Indhold = "Fælles formål aftalt: vi vil forstå Emmas hverdag og finde et lille skridt, der gør skolen lettere." },
                                new() { Fase = "BeskrivObservation", Perspektiv = "Skole", Indhold = "Konkrete observationer delt: sen ankomst i matematik, gik hjem efter idræt, men stærk i naturfagsgruppen." },
                                new() { Fase = "InviterHjemmetsPerspektiv", Perspektiv = "Hjem", Indhold = "Mor fortæller: Emma sover dårligt om søndagen, klager over mavepine om morgenen og bliver lettet, når hun ikke skal af sted." },
                                new() { Fase = "ElevensPerspektiv", Perspektiv = "Elev", Indhold = "Emma: idræt og store fora er svære; føler sig set i naturfagsgruppen og vil gerne have en fast voksen at gå til om morgenen." },
                                new() { Fase = "FaellesForstaaelse", Perspektiv = "Skole", Indhold = "Fælles billede: ubehag op til og i udvalgte situationer; aftaler at undersøge, om der er noget socialt i idrætten." },
                                new() { Fase = "Aftaler", Perspektiv = "Skole", Indhold = "Aftalt: fast kontaktperson om morgenen, gradvis deltagelse i idræt, mor deler morgenmønster." },
                                new() { Fase = "StemmeTjek", Perspektiv = "Skole", Indhold = "Oplevede begge sig hørt; Emma siger, det var rart at blive spurgt. Næste samtale om 3 uger." }
                            },
                            Aftaler = new List<SeedAftale>
                            {
                                new() { Beskrivelse = "Fast kontaktperson tager imod Emma ved morgensamlingen.", Ansvarlig = "Skole", Frist = D(-1), Evalueringsdato = D(-21) },
                                new() { Beskrivelse = "Hjemmet noterer søvn og morgenhumør i en uge og deler med skolen.", Ansvarlig = "Hjem", Evalueringsdato = D(-21) },
                                new() { Beskrivelse = "Emma vælger selv, hvilke to dage hun deltager i idræt i næste uge.", Ansvarlig = "Elev", Evalueringsdato = D(-21) }
                            },
                            HjemmeIndsigter = new List<SeedHjemmeIndsigt>
                            {
                                new() { Spoergsmaal = "Hvordan er aftenerne og morgenerne typisk?", Kategori = "Trivsel", BidragetAf = "Mor (Anne)", Svar = "Sover først sent om søndagen. Klager over mavepine om morgenen og spørger, om hun må blive hjemme." },
                                new() { Spoergsmaal = "Hvad fortæller eleven derhjemme om skole, kammerater og pauser?", Kategori = "Social", BidragetAf = "Mor (Anne)", Svar = "Taler om idræt som det sværeste – føler sig holdt udenfor, når der skal vælges hold." }
                            }
                        }
                    }
                },
                new()
                {
                    Navn = "Noah Lind",
                    Klasse = "7B",
                    Observationer = new List<SeedObservation>
                    {
                        new()
                        {
                            Dato = D(6), Kategori = "Social", Valens = "Opmærksomhed",
                            Beskrivelse = "Afbrød flere gange i gruppearbejdet og kom i konflikt med en klassekammerat om en opgave.",
                            Kontekst = "Dansk, gruppearbejde", ObserveretAf = "Lærer (undertegnede)"
                        },
                        new()
                        {
                            Dato = D(4), Kategori = "Faglig", Valens = "Styrke",
                            Beskrivelse = "Fremlagde et gennemarbejdet oplæg og svarede roligt på spørgsmål.",
                            Kontekst = "Samfundsfag", ObserveretAf = "Lærer (undertegnede)", DeltMedHjem = true
                        }
                    },
                    Samtaler = new List<SeedSamtale>
                    {
                        new()
                        {
                            Type = "Foraeldresamtale", Dato = D(-4),
                            Formaal = "Følge op på observationer om samarbejde i grupper og lytte til hjemmets og Noahs eget perspektiv.",
                            SkabelonKode = "skole-hjem-grund", Status = "Planlagt",
                            Deltagere = new List<SeedDeltager>
                            {
                                new() { Navn = "Far (Peter)", Rolle = "Foraelder" },
                                new() { Navn = "Kontaktlærer", Rolle = "Laerer" }
                            },
                            Noter = new List<SeedNote>
                            {
                                new() { Fase = "Forberedelse", Perspektiv = "Skole", Indhold = "Har to konkrete observationer klar – én opmærksomhed og én styrke." }
                            }
                        }
                    }
                }
            }
        };
    }
}
