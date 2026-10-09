using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Framework;

public sealed record HjemmeSpoergsmaal(string Spoergsmaal, ObservationKategori Kategori);

/// <summary>
/// Spørgeramme der hjælper læreren med at indsamle den viden fra hjemmet,
/// som ikke er tilgængelig i skolekonteksten (søvn, morgener, helbred, fritid,
/// hvad eleven siger derhjemme). Spørgsmålene er neutralt formulerede og
/// behandler hjemmets svar som perspektiv, ikke som bevis på årsag.
/// </summary>
public static class HjemmeSpoergeramme
{
    public static readonly IReadOnlyList<HjemmeSpoergsmaal> Alle = new[]
    {
        new HjemmeSpoergsmaal("Hvordan er aftenerne og morgenerne typisk – søvn, tidspunkt og hvor lang tid tager det at komme af sted?", ObservationKategori.Trivsel),
        new HjemmeSpoergsmaal("Klager eleven over ondt i maven, hovedpine eller andre fysiske gener i forbindelse med skole?", ObservationKategori.Trivsel),
        new HjemmeSpoergsmaal("Hvornår begyndte mønsteret, og varierer det efter ugedag, fag eller særlige situationer?", ObservationKategori.Deltagelse),
        new HjemmeSpoergsmaal("Hvad fortæller eleven derhjemme om skole, kammerater, voksne og pauser?", ObservationKategori.Social),
        new HjemmeSpoergsmaal("Hvad sker der typisk efter en dag, hvor eleven har haft det svært, eller har været væk?", ObservationKategori.Fravær),
        new HjemmeSpoergsmaal("Er der sket ændringer i familien, helbred, venskaber, transport eller rutiner, som I ønsker, vi skal kende til?", ObservationKategori.Trivsel),
        new HjemmeSpoergsmaal("Hvad har I allerede prøvet – og hvad hjalp, også hvis det kun hjalp lidt eller kortvarigt?", ObservationKategori.Trivsel),
        new HjemmeSpoergsmaal("Hvilke aktiviteter eller opgaver i skolen glæder eleven sig til eller taler positivt om?", ObservationKategori.Faglig)
    };

    public static IEnumerable<HjemmeSpoergsmaal> ForKategori(ObservationKategori kategori)
        => Alle.Where(s => s.Kategori == kategori);
}
