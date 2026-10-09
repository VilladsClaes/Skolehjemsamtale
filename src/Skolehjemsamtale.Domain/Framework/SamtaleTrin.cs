using Skolehjemsamtale.Domain.Enums;

namespace Skolehjemsamtale.Domain.Framework;

/// <summary>Et trin i samtalestrukturen med vejledende spørgsmål og opmærksomhedspunkter.</summary>
public sealed record SamtaleTrin(
    SamtaleFaseKode Kode,
    string Titel,
    string Formaal,
    Perspektiv PrimaertPerspektiv,
    IReadOnlyList<string> Spoergsmaal,
    IReadOnlyList<string> PasPaa);

/// <summary>En navngivet samtalestruktur (skabelon), der kan anvendes på en samtale.</summary>
public sealed record SamtaleSkabelon(
    string Kode,
    string Navn,
    string Beskrivelse,
    SamtaleType AnbefaletType,
    IReadOnlyList<SamtaleTrin> Trin);
