using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;
using BCrypt.Net;
using System.Linq;
using Spectre.Console;



public class Produkt
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public decimal Cena { get; set; }
    public string Opis { get; set; }
    public int Ilosc { get; set; }
}

public class Uzytkownik
{
    public string Nazwa { get; set; }
    public string Haslo { get; set; }
    public bool IsAdmin { get; set; }

    public Uzytkownik() { }

    public Uzytkownik(string nazwa, string haslo, bool isAdmin = false)
    {
        Nazwa = nazwa;
        Haslo = haslo;
        IsAdmin = isAdmin;
    }
}


public class Koszyk
{
    public List<Produkt> Produkty { get; } = new List<Produkt>();

    public void DodajProdukt(Produkt produkt, int ilosc = 1)
    {
        produkt.Ilosc = ilosc;
        AnsiConsole.Status().Start($"Dodawanie {produkt.Nazwa} (ilość: {ilosc}) do koszyka...", ctx =>
        {
            ctx.Spinner(Spinner.Known.Star);
            Task.Delay(1000).Wait();
        });

        Produkty.Add(produkt);
        AnsiConsole.MarkupLine($"[green]Dodano {produkt.Nazwa} (ilość: {ilosc}) do koszyka![/]");
    }

    public void UsunProdukt(Produkt produkt)
    {
        if (Produkty.Contains(produkt))
        {
            Produkty.Remove(produkt);
            Console.Clear();
            AnsiConsole.MarkupLine($"[red]Usunięto {produkt.Nazwa} z koszyka![/]");
            WyswietlKoszyk();

        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]Produkt nie znajduje się w koszyku![/]");
        }
    }

    public void WyczyscKoszyk()
    {
        Produkty.Clear();
        AnsiConsole.MarkupLine("[yellow]Koszyk został wyczyszczony![/]");
    }

    public void WyswietlKoszyk()
    {
        if (Produkty.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Koszyk jest pusty![/]");
        }
        else
        {
            var tabela = new Table().AddColumn("ID").AddColumn("Nazwa").AddColumn("Cena").AddColumn("Ilość");
            foreach (var produkt in Produkty)
            {
                tabela.AddRow(produkt.Id.ToString(), produkt.Nazwa, produkt.Cena.ToString("C"), produkt.Ilosc.ToString());
            }
            AnsiConsole.Write(tabela);
        }
    }

}



public class UzytkownikModel
{
    private readonly string _connectionString = "Data Source=sklep.db;Version=3;";
    public Uzytkownik ZalogowanyUzytkownik { get; private set; }

    public UzytkownikModel()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS uzytkownicy (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nazwa TEXT NOT NULL,
                        Haslo TEXT NOT NULL,
                        IsAdmin INTEGER DEFAULT 0
                    )");
        }
    }

    public bool ZarejestrujUzytkownika(string nazwa, string haslo)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(haslo);
            connection.Execute("INSERT INTO uzytkownicy (Nazwa, Haslo) VALUES (@Nazwa, @Haslo)",
                new { Nazwa = nazwa, Haslo = hashedPassword });
            return true;
        }
    }

    public bool ZalogujUzytkownika(string nazwa, string haslo)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            var user = connection.QueryFirstOrDefault<Uzytkownik>(
                "SELECT * FROM uzytkownicy WHERE Nazwa = @Nazwa",
                new { Nazwa = nazwa });
            if (user != null && BCrypt.Net.BCrypt.Verify(haslo, user.Haslo))
            {
                ZalogowanyUzytkownik = user;
                return true;
            }
            return false;
        }
    }

    public void Wyloguj()
    {
        ZalogowanyUzytkownik = null;
    }
}

public class ProduktModel
{
    private readonly string _connectionString = "Data Source=sklep.db;Version=3;";

    public ProduktModel()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Execute(@"
                    CREATE TABLE IF NOT EXISTS produkty (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nazwa TEXT NOT NULL,
                        Cena REAL NOT NULL,
                        Opis TEXT,
                        Ilosc INTEGER DEFAULT 1
                    )");
        }
    }

    public List<Produkt> PobierzWszystkieProdukty()
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            return connection.Query<Produkt>("SELECT * FROM produkty").ToList();
        }
    }

    public void DodajProdukt(Produkt produkt)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Execute("INSERT INTO produkty (Nazwa, Cena, Opis, Ilosc) VALUES (@Nazwa, @Cena, @Opis, @Ilosc)",
                new { produkt.Nazwa, produkt.Cena, produkt.Opis, produkt.Ilosc });
        }
    }

    public void UsunProdukt(int id)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Execute("DELETE FROM produkty WHERE Id = @Id", new { Id = id });
        }
    }

    public void EdytujProdukt(Produkt produkt)
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Execute(
                "UPDATE produkty SET Nazwa = @Nazwa, Cena = @Cena, Opis = @Opis, Ilosc = @Ilosc WHERE Id = @Id",
                new { produkt.Nazwa, produkt.Cena, produkt.Opis, produkt.Ilosc, produkt.Id });
        }
    }
}