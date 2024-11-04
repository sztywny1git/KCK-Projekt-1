using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;


public static class View
{
    public static void WyswietlWitaj()
    {
        AnsiConsole.MarkupLine("[bold cyan]Witaj w Sklepie Internetowym z Elektroniką![/]");

        AnsiConsole.Status().Start("Trwa ładowanie...", ctx =>
        {
            ctx.Spinner(Spinner.Known.Dots);
            Task.Delay(1500).Wait();
        });

        AnsiConsole.MarkupLine("[green]Zaloguj się lub zarejestruj, aby kontynuować.[/]");

    }

    public static void WyswietlMenuLogowania()
    {
        var menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(new[] { "Zaloguj się", "Zarejestruj się", "Wyjdź" }));
        Program.controller.WybierzAkcjeLogowania(menu);
    }

    public static void WyswietlMenuGlowne()
    {
        var menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Wybierz opcję:")
                .AddChoices(new[] { "Przeglądaj produkty", "Dodaj do koszyka", "Usuń z koszyka", "Wyświetl koszyk", "Wyczyść koszyk", "Wyloguj się", "Wyjdź" }));
        Program.controller.WybierzAkcjeGlowne(menu);
    }


    public static void WyswietlProdukty(List<Produkt> produkty)
    {
        Console.Clear();
        var tabela = new Table().AddColumn("ID").AddColumn("Nazwa").AddColumn("Cena").AddColumn("Opis");
        foreach (var produkt in produkty)
        {
            tabela.AddRow(produkt.Id.ToString(), produkt.Nazwa, produkt.Cena.ToString("C"), produkt.Opis);
        }
        AnsiConsole.Write(tabela);
    }

    public static void WyswietlProduktyZAdmina(List<Produkt> produkty)
    {
        Console.Clear();
        var tabela = new Table().AddColumn("ID").AddColumn("Nazwa").AddColumn("Cena").AddColumn("Opis");
        foreach (var produkt in produkty)
        {
            tabela.AddRow(produkt.Id.ToString(), produkt.Nazwa, produkt.Cena.ToString("C"), produkt.Opis);
        }
        AnsiConsole.Write(tabela);
    }

    public static void WyswietlMenuAdmina()
    {
        var menu = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Menu Administratora:")
                .AddChoices(new[] { "Dodaj produkt", "Edytuj produkt", "Usuń produkt", "Przeglądaj produkty", "Wyloguj się" }));
        Program.controller.WybierzAkcjeAdmina(menu);
    }
}
