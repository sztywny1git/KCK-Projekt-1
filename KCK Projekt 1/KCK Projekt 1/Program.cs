using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Spectre.Console;


public class Program
{
    public static UzytkownikModel _uzytkownikModel;
    public static ProduktModel _produktModel;
    public static Koszyk _koszyk;
    public static Controller controller;

    static void Main(string[] args)
    {
        _uzytkownikModel = new UzytkownikModel();
        _produktModel = new ProduktModel();
        _koszyk = new Koszyk();
        controller = new Controller(_uzytkownikModel, _produktModel, _koszyk);

        View.WyswietlWitaj();
        View.WyswietlMenuLogowania();
    }
}

