namespace Skolehjemsamtale.Infrastructure.Seed;

/// <summary>
/// Filformat for import af eksisterende data fra samtaleværktøjet (samtale_vaerktoej_data.json).
/// Bevidst tolerant og versionsstyret, så en tidligere export kan indlæses uden kodeændringer.
/// </summary>
public class SeedRoot
{
    public int Version { get; set; } = 1;
    public List<SeedElev> Elever { get; set; } = new();
}

public class SeedElev
{
    public string Navn { get; set; } = string.Empty;
    public string Klasse { get; set; } = string.Empty;
    public string? Foedselsdato { get; set; }
    public List<SeedObservation> Observationer { get; set; } = new();
    public List<SeedSamtale> Samtaler { get; set; } = new();
}

public class SeedObservation
{
    public string? Dato { get; set; }
    public string Kategori { get; set; } = "Trivsel";
    public string Valens { get; set; } = "Opmærksomhed";
    public string Beskrivelse { get; set; } = string.Empty;
    public string Kontekst { get; set; } = string.Empty;
    public string ObserveretAf { get; set; } = "Lærer";
    public string? HvadHjalp { get; set; }
    public bool DeltMedHjem { get; set; }
}

public class SeedSamtale
{
    public string Type { get; set; } = "SkoleHjemSamtale";
    public string? Dato { get; set; }
    public string Formaal { get; set; } = string.Empty;
    public string SkabelonKode { get; set; } = "skole-hjem-grund";
    public string? Sted { get; set; }
    public string Status { get; set; } = "Planlagt";
    public List<SeedDeltager> Deltagere { get; set; } = new();
    public List<SeedNote> Noter { get; set; } = new();
    public List<SeedAftale> Aftaler { get; set; } = new();
    public List<SeedHjemmeIndsigt> HjemmeIndsigter { get; set; } = new();
}

public class SeedDeltager
{
    public string Navn { get; set; } = string.Empty;
    public string Rolle { get; set; } = "Foraelder";
}

public class SeedNote
{
    public string Fase { get; set; } = "Aabning";
    public string Perspektiv { get; set; } = "Skole";
    public string Indhold { get; set; } = string.Empty;
}

public class SeedAftale
{
    public string Beskrivelse { get; set; } = string.Empty;
    public string Ansvarlig { get; set; } = "Faelles";
    public string? AnsvarligNavn { get; set; }
    public string? Frist { get; set; }
    public string? Evalueringsdato { get; set; }
}

public class SeedHjemmeIndsigt
{
    public string Spoergsmaal { get; set; } = string.Empty;
    public string Svar { get; set; } = string.Empty;
    public string Kategori { get; set; } = "Trivsel";
    public string BidragetAf { get; set; } = string.Empty;
}
