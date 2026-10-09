namespace Skolehjemsamtale.Domain.Enums;

/// <summary>Hvilket perspektiv en oplysning kommer fra. Bruges til at holde skole, hjem og elev adskilt.</summary>
public enum Perspektiv
{
    Skole = 1,
    Hjem = 2,
    Elev = 3
}

/// <summary>Overordnet kategori for en observation af elevens adfærd i undervisningen.</summary>
public enum ObservationKategori
{
    Faglig = 1,
    Social = 2,
    Trivsel = 3,
    Adfaerd = 4,
    Deltagelse = 5,
    Fravær = 6
}

/// <summary>
/// Om en observation beskriver en styrke/resource eller noget, der kræver opmærksomhed.
/// Værktøjet skal kunne løfte begge sider – ikke kun problemer.
/// </summary>
public enum ObservationValens
{
    Styrke = 1,
    Opmærksomhed = 2
}

public enum SamtaleType
{
    Foraeldresamtale = 1,
    Elevsamtale = 2,
    SkoleHjemSamtale = 3,
    Trefoldighedssamtale = 4,
    Opfoelgning = 5
}

public enum SamtaleStatus
{
    Planlagt = 1,
    Afholdt = 2,
    Afsluttet = 3,
    Aflyst = 4
}

public enum SamtaleFaseKode
{
    Forberedelse = 1,
    Aabning = 2,
    BeskrivObservation = 3,
    InviterHjemmetsPerspektiv = 4,
    ElevensPerspektiv = 5,
    FaellesForstaaelse = 6,
    Aftaler = 7,
    StemmeTjek = 8
}

public enum SamtaleRolle
{
    Elev = 1,
    Foraelder = 2,
    Laerer = 3,
    Paedagog = 4,
    Ledelse = 5,
    Ekstern = 6
}

public enum AftaleAnsvar
{
    Skole = 1,
    Hjem = 2,
    Elev = 3,
    Faelles = 4
}

public enum AftaleStatus
{
    Foreslaaet = 1,
    Aktiv = 2,
    Afsluttet = 3,
    Afbrudt = 4
}
