using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;


public class Controller
{
    private UzytkownikModel _uzytkownikModel;
    private ProduktModel _produktModel;
    private Koszyk _koszyk;

    public Controller(UzytkownikModel uzytkownikModel, ProduktModel produktModel, Koszyk koszyk)
    {
        _uzytkownikModel = uzytkownikModel;
        _produktModel = produktModel;
        _koszyk = koszyk;
    }

    public void DodajDoKoszyka()
    {
        var produkty = _produktModel.PobierzWszystkieProdukty();
        View.WyswietlProdukty(produkty);

        var id = AnsiConsole.Ask<int>("Podaj ID produktu do dodania do koszyka:");
        var produkt = produkty.FirstOrDefault(p => p.Id == id);

        if (produkt != null)
        {
            var ilosc = AnsiConsole.Ask<int>("Podaj ilość:");
            _koszyk.DodajProdukt(produkt, ilosc);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Nie znaleziono produktu o podanym ID![/]");
        }
        Console.Clear();
    }

    public void UsunZKoszyka()
    {
        if (_koszyk.Produkty.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Koszyk jest pusty! Nie można usunąć produktu.[/]");
            return;
        }

        _koszyk.WyswietlKoszyk();
        var id = AnsiConsole.Ask<int>("Podaj ID produktu do usunięcia z koszyka:");
        var produkt = _koszyk.Produkty.FirstOrDefault(p => p.Id == id);

        if (produkt != null)
        {
            var iloscDoUsuniecia = AnsiConsole.Ask<int>("Podaj ilość do usunięcia:");
            if (iloscDoUsuniecia >= produkt.Ilosc)
            {
                _koszyk.UsunProdukt(produkt);
            }
            else
            {
                Console.Clear();
                produkt.Ilosc -= iloscDoUsuniecia;
                AnsiConsole.MarkupLine($"[yellow]Zredukowano ilość {produkt.Nazwa} o {iloscDoUsuniecia}. Nowa ilość: {produkt.Ilosc}.[/]");
                _koszyk.WyswietlKoszyk();
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Produkt nie został znaleziony w koszyku![/]");
        }
    }



    public void WyczyscKoszyk()
    {
        _koszyk.WyczyscKoszyk();
    }

    public void WybierzAkcjeLogowania(string akcja)
    {
        switch (akcja)
        {
            case "Zaloguj się":
                Zaloguj();
                break;
            case "Zarejestruj się":
                Zarejestruj();
                break;
            case "Wyjdź":
                Environment.Exit(0);
                break;
        }
    }

    private void Zaloguj()
    {
        var nazwa = AnsiConsole.Ask<string>("Podaj nazwę użytkownika:");
        var haslo = AnsiConsole.Prompt(new TextPrompt<string>("Podaj hasło:").PromptStyle("green").Secret());

        if (_uzytkownikModel.ZalogujUzytkownika(nazwa, haslo))
        {
            AnsiConsole.MarkupLine("[green]Zalogowano pomyślnie![/]");
            Console.Clear();

            // Create a new Koszyk for the logged-in user
            _koszyk = new Koszyk();

            if (_uzytkownikModel.ZalogowanyUzytkownik.IsAdmin)
            {
                View.WyswietlMenuAdmina();
                var akcja = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Menu Administratora:")
                        .AddChoices(new[] { "Dodaj produkt", "Edytuj produkt", "Usuń produkt", "Przeglądaj produkty", "Wyloguj się" }));
                WybierzAkcjeAdmina(akcja);
            }
            else
            {
                View.WyswietlMenuGlowne();
                var akcja = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wybierz opcję:")
                        .AddChoices(new[] { "Przeglądaj produkty", "Dodaj do koszyka", "Usuń z koszyka", "Wyświetl koszyk", "Wyczyść koszyk", "Wyloguj się", "Wyjdź" }));
                WybierzAkcjeGlowne(akcja);
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Błędna nazwa użytkownika lub hasło![/]");
            View.WyswietlMenuLogowania();
        }
    }


    private void Zarejestruj()
    {
        var nazwa = AnsiConsole.Ask<string>("Podaj nazwę użytkownika:");
        var haslo = AnsiConsole.Prompt(new TextPrompt<string>("Podaj hasło:").PromptStyle("green").Secret());

        if (_uzytkownikModel.ZarejestrujUzytkownika(nazwa, haslo))
        {
            AnsiConsole.MarkupLine("[green]Zarejestrowano pomyślnie![/]");
            View.WyswietlMenuLogowania();
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Użytkownik o tej nazwie już istnieje![/]");
            View.WyswietlMenuLogowania();
        }
    }

    public void WybierzAkcjeGlowne(string akcja)
    {
        while (true)
        {
            try
            {
                Console.Clear();
                switch (akcja)
                {
                    case "Przeglądaj produkty":
                        var produkty = _produktModel.PobierzWszystkieProdukty();
                        View.WyswietlProdukty(produkty);
                        break;
                    case "Dodaj do koszyka":
                        DodajDoKoszyka();
                        _koszyk.WyswietlKoszyk();
                        break;
                    case "Usuń z koszyka":
                        UsunZKoszyka();
                        break;
                    case "Wyświetl koszyk":
                        _koszyk.WyswietlKoszyk();
                        break;
                    case "Wyczyść koszyk":
                        WyczyscKoszyk();
                        break;
                    case "Wyloguj się":
                        Console.Clear();
                        _uzytkownikModel.Wyloguj();
                        AnsiConsole.MarkupLine("[green]Wylogowano pomyślnie![/]");
                        View.WyswietlMenuLogowania();
                        return;
                    case "Wyjdź":
                        Environment.Exit(0);
                        break;
                }

                var wybor = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Wybierz opcję:")
                        .AddChoices(new[] { "Przeglądaj produkty", "Dodaj do koszyka", "Usuń z koszyka", "Wyświetl koszyk", "Wyczyść koszyk", "Wyloguj się", "Wyjdź" }));
                akcja = wybor;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Wystąpił błąd: {ex.Message}[/]");
                break;
            }
        }
    }

    public void WybierzAkcjeAdmina(string akcja)
    {
        while (true)
        {
            try
            {
                Console.Clear();
                switch (akcja)
                {
                    case "Dodaj produkt":
                        DodajProdukt();
                        break;
                    case "Edytuj produkt":
                        EdytujProdukt();
                        break;
                    case "Usuń produkt":
                        UsunProdukt();
                        break;
                    case "Przeglądaj produkty":
                        var produkty = _produktModel.PobierzWszystkieProdukty();
                        View.WyswietlProduktyZAdmina(produkty);
                        break;
                    case "Wyloguj się":
                        Console.Clear();
                        _uzytkownikModel.Wyloguj();
                        AnsiConsole.MarkupLine("[green]Wylogowano pomyślnie![/]");
                        View.WyswietlMenuLogowania();
                        return;
                }

                var wybor = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Menu Administratora:")
                        .AddChoices(new[] { "Dodaj produkt", "Edytuj produkt", "Usuń produkt", "Przeglądaj produkty", "Wyloguj się" }));
                akcja = wybor;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Wystąpił błąd: {ex.Message}[/]");
                break;
            }
        }
    }

    private void DodajProdukt()
    {
        var nazwa = AnsiConsole.Ask<string>("Podaj nazwę produktu:");
        var cena = AnsiConsole.Ask<decimal>("Podaj cenę produktu:");
        var opis = AnsiConsole.Ask<string>("Podaj opis produktu:");

        var produkt = new Produkt { Nazwa = nazwa, Cena = cena, Opis = opis };
        _produktModel.DodajProdukt(produkt);
        AnsiConsole.MarkupLine("[green]Produkt dodany pomyślnie![/]");
        Console.Clear();
    }

    private void EdytujProdukt()
    {
        var produkty = _produktModel.PobierzWszystkieProdukty();
        View.WyswietlProduktyZAdmina(produkty);

        var id = AnsiConsole.Ask<int>("Podaj ID produktu do edycji:");
        var nazwa = AnsiConsole.Ask<string>("Podaj nową nazwę produktu:");
        var cena = AnsiConsole.Ask<decimal>("Podaj nową cenę produktu:");
        var opis = AnsiConsole.Ask<string>("Podaj nowy opis produktu:");

        var produkt = new Produkt { Id = id, Nazwa = nazwa, Cena = cena, Opis = opis };
        _produktModel.EdytujProdukt(produkt);
        AnsiConsole.MarkupLine("[green]Produkt edytowany pomyślnie![/]");
        Console.Clear();
    }

    private void UsunProdukt()
    {
        var produkty = _produktModel.PobierzWszystkieProdukty();
        View.WyswietlProduktyZAdmina(produkty);

        var id = AnsiConsole.Ask<int>("Podaj ID produktu do usunięcia:");
        _produktModel.UsunProdukt(id);
        AnsiConsole.MarkupLine("[red]Produkt usunięty pomyślnie![/]");
        Console.Clear();
    }
}
