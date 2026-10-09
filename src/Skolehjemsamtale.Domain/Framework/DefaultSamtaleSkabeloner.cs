using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Framework;

/// <summary>
/// De indbyggede samtalestrukturer. Indholdet er udledt af forskning i skolehjemsamarbejde,
/// skolevægring/bekymrende fravær samt feedback- og samtalekultur (se docs/research).
/// Principperne: beskriv frem for at dømme, inddrag frem for at informere, hold perspektiver adskilt,
/// aftal små testbare skridt, og tjek at parterne oplevede sig hørt.
/// </summary>
public static class DefaultSamtaleSkabeloner
{
    public const string SkoleHjemGrund = "skole-hjem-grund";
    public const string SkolevaegringOpfoelgning = "skolevaegring-opfoelgning";
    public const string Elevsamtale = "elevsamtale";

    private static readonly SamtaleTrin Forberedelse = new(
        SamtaleFaseKode.Forberedelse,
        "Forbered dig",
        "Klargør formål, deltagere og de konkrete observationer – både styrker og opmærksomheder – samt hvad der allerede har hjulpet.",
        Perspektiv.Skole,
        new[]
        {
            "Hvad er formålet med samtalen, formuleret i én sætning?",
            "Hvilke konkrete observationer lægger jeg frem (hvornår, hvor, hvad)?",
            "Hvilke styrker og ressourcer skal også frem?",
            "Hvad har allerede hjulpet – også kortvarigt?",
            "Skal eleven deltage, og på hvilken måde vil eleven helst bidrage?"
        },
        new[]
        {
            "Undgå at gøre samtalen til en afrapportering eller dom over eleven eller familien.",
            "Tag ikke for givet, at dine observationer forklarer årsagen.",
            "Lad være med først at kontakte hjemmet, når noget er gået galt."
        });

    private static readonly SamtaleTrin Aabning = new(
        SamtaleFaseKode.Aabning,
        "Åbn med partnerskab",
        "Sæt den fælles hensigt: vi vil forstå, hvad der sker for eleven, og finde ud af sammen, hvad der kan hjælpe.",
        Perspektiv.Skole,
        new[]
        {
            "Hvad ønsker I mest, at skolen forstår om jeres barn lige nu?",
            "Hvad ser I, som vi i skolen måske ikke har set?",
            "Hvordan kan vi sammen skabe den bedste ramme om denne samtale?"
        },
        new[]
        {
            "Start med lyttende spørgsmål frem for forklaringer og ansvarsfordeling.",
            "Gør roller og forventninger tydelige, så ansvaret ikke skrider.",
            "Vælg den talt form frem for en lang skriftlig besked ved følsomme emner."
        });

    private static readonly SamtaleTrin BeskrivObservation = new(
        SamtaleFaseKode.BeskrivObservation,
        "Beskriv – døm ikke",
        "Fremlæg specifikke observationer fra undervisningen med situation, adfærd og betydning (SBI), uden mærkater eller tillagt motiv.",
        Perspektiv.Skole,
        new[]
        {
            "I [situation] lagde jeg mærke til [observerbar adfærd].",
            "Det betød for undervisningen/gruppen, at [betydning].",
            "Hvad var du/i gang med at opnå i den situation? (SBII – undersøg hensigt)",
            "Hvordan ser det ud fra jeres side, når I hører det?"
        },
        new[]
        {
            "Brug ikke ord som 'doven', 'urolig', 'manipulerende', 'altid' eller 'aldrig'.",
            "Adskil det observerede fra den historie, du fortæller dig selv om hvorfor.",
            "Tal om adfærd og betydning – ikke om personen eller familiens måde at være på."
        });

    private static readonly SamtaleTrin InviterHjemmetsPerspektiv = new(
        SamtaleFaseKode.InviterHjemmetsPerspektiv,
        "Inviter hjemmets perspektiv",
        "Lyt til forældrenes oplevelse uden at afbryde, opsummer og tjek forståelsen. Hjemmet kan se noget, skolen ikke kan.",
        Perspektiv.Hjem,
        new[]
        {
            "Hvad lægger I mærke til derhjemme – i aftener og morgener?",
            "Hvornår er det lettere, og hvornår er det sværere?",
            "Hvad siger eleven derhjemme om skole, kammerater og voksne?",
            "Hvad tror I, vi i skolen kan have overset?",
            "Hvad har I allerede prøvet, og hvad hjalp – også hvis det kun hjalp lidt?"
        },
        new[]
        {
            "Afbryd ikke, og gå ikke i forsvar, når noget ikke stemmer med skolens billede.",
            "Behandl forskelle som noget, vi undersøger sammen – ikke som en modstrid.",
            "Brug hjemmets oplysninger som perspektiv på elevens oplevelse, ikke som bevis på årsag."
        });

    private static readonly SamtaleTrin ElevensPerspektiv = new(
        SamtaleFaseKode.ElevensPerspektiv,
        "Elevens stemme",
        "Spørg eleven, hvordan eleven vil have sin stemme hørt, og hvad der ville gøre skolen lettere – uden at gøre eleven ansvarlig for de voksnes plan.",
        Perspektiv.Elev,
        new[]
        {
            "Hvordan har du det med at gå i skole for tiden?",
            "Hvilke dele af skoledagen er lettest, og hvornår bliver det svært?",
            "Hvad sker der lige før, du ikke har lyst til at være her?",
            "Hvad ville gøre det næste skridt i skole lettere for dig?",
            "Hvordan vil du helst have, at vi voksne taler om det – og hvad må vi dele?"
        },
        new[]
        {
            "Gør eleven til deltager i eget liv – ikke blot til emnet, de voksne taler om.",
            "Giv eleven valgmuligheder og tid; kræv ikke, at alt forklares på én gang.",
            "Læg ikke ansvaret for at løse skolens udfordring på eleven alene."
        });

    private static readonly SamtaleTrin FaellesForstaaelse = new(
        SamtaleFaseKode.FaellesForstaaelse,
        "Fælles billede",
        "Sammenlign skolens, hjemmets og elevens billede. Skeln mellem det observerede, det rapporterede og det, der stadig er en hypotese.",
        Perspektiv.Skole,
        new[]
        {
            "Hvad er vi enige om, at vi ser?",
            "Hvad ser vi forskelligt – og hvad kan forklare forskellen?",
            "Hvilke barrierer gør det svært, og hvilke støtter gør det lettere?",
            "Er der noget omkring trivsel, tryghed eller læring, vi skal undersøge nærmere?"
        },
        new[]
        {
            "Undgå at lade det ene billede vinde som det 'rigtige'.",
            "Bliv ikke i problemet – led samtidig efter ressourcer og muligheder.",
            "Vær opmærksom på tegn på mistrivsel eller bekymrende fravær, der kræver særskilt handling."
        });

    private static readonly SamtaleTrin Aftaler = new(
        SamtaleFaseKode.Aftaler,
        "Aftal små, testbare skridt",
        "Beslut hvad skolen, hjemmet og eleven hver især gør, hvornår det sker, hvem der er kontaktperson, og hvornår vi følger op.",
        Perspektiv.Skole,
        new[]
        {
            "Hvad er det første, lille skridt, vi kan prøve?",
            "Hvad tager skolen ansvar for, og hvad kan hjemmet og eleven bidrage med?",
            "Hvem er fast kontaktperson, og hvordan holder vi kontakt?",
            "Hvornår mødes eller taler vi igen for at se, om det virkede?"
        },
        new[]
        {
            "Lad ikke ansvaret blive ensidigt eller uklart – skriv det eksplicit.",
            "Lov ikke hurtige løsninger, og undgå en alt-for-stor første ændring.",
            "Aftal en dato for evaluering – ikke kun en intention."
        });

    private static readonly SamtaleTrin StemmeTjek = new(
        SamtaleFaseKode.StemmeTjek,
        "Stemmetjek og afrunding",
        "Tjek at deltagerne oplevede at blive hørt og forstået, og hvad der stadig er uafklaret. Beslut hvad eleven får at vide bagefter.",
        Perspektiv.Skole,
        new[]
        {
            "Oplevede du/din bekymring sig forstået i dag?",
            "Hvad har været brugbart, og hvad er stadig uafklaret?",
            "Hvad fortæller vi videre – og til hvem?",
            "Hvordan har eleven det med at gå herfra?"
        },
        new[]
        {
            "Spring ikke stemmetjekket over, selv om samtalen har været svær.",
            "Undlad at love mere, end I kan holde.",
            "Vær opmærksom på, at ændringer kan tage tid – aftal opfølgning frem for at konkludere."
        });

    public static readonly SamtaleSkabelon SkoleHjemGrundSkabelon = new(
        SkoleHjemGrund,
        "Skole-hjem-samtale (grund)",
        "Ærlig, vækstorienteret samtale med forældre og elev om observationer fra undervisningen.",
        SamtaleType.SkoleHjemSamtale,
        new[] { Forberedelse, Aabning, BeskrivObservation, InviterHjemmetsPerspektiv, ElevensPerspektiv, FaellesForstaaelse, Aftaler, StemmeTjek });

    public static readonly SamtaleSkabelon SkolevaegringSkabelon = new(
        SkolevaegringOpfoelgning,
        "Opfølgning på skolevægring / bekymrende fravær",
        "Samtale der tidligt undersøger mønstre i fravær, kortlægger barrierer og støtter og aftaler en trinvis vej tilbage til skole.",
        SamtaleType.SkoleHjemSamtale,
        new[] { Forberedelse, Aabning, BeskrivObservation, InviterHjemmetsPerspektiv, ElevensPerspektiv, FaellesForstaaelse, Aftaler, StemmeTjek });

    public static readonly SamtaleSkabelon ElevsamtaleSkabelon = new(
        Elevsamtale,
        "Elevsamtale",
        "Samtale hvor elevens egen stemme er i centrum, og hvor eleven får ejerskab over næste skridt.",
        SamtaleType.Elevsamtale,
        new[] { Forberedelse, Aabning, BeskrivObservation, ElevensPerspektiv, FaellesForstaaelse, Aftaler, StemmeTjek });

    public static IReadOnlyList<SamtaleSkabelon> Alle => new[] { SkoleHjemGrundSkabelon, SkolevaegringSkabelon, ElevsamtaleSkabelon };

    public static SamtaleSkabelon Find(string kode) =>
        Alle.FirstOrDefault(s => string.Equals(s.Kode, kode, StringComparison.OrdinalIgnoreCase))
        ?? SkoleHjemGrundSkabelon;
}
