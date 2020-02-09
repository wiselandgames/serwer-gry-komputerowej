using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsługaSerwera
{
    public static void PowitanieOtrzymane(int _odKlienta, Pakiet _pakiet)
    {
        int _sprawdźIdKlienta = _pakiet.ReadInt();
        string _nazwaloginu = _pakiet.ReadString();

        Debug.Log($"{Serwer.klienci[_odKlienta].tcp.gniazdo.Client.RemoteEndPoint} połączył się z powodzeniem i teraz jest graczem {_odKlienta}.");
        if (_odKlienta != _sprawdźIdKlienta)
        {
            Debug.Log($"Gracz \" {_nazwaloginu }\"(ID: {_odKlienta}) otrzymał złe ID klienta ({_sprawdźIdKlienta})!");
        }
        Serwer.klienci[_odKlienta].WyślijDoGry(_nazwaloginu);
    }
    public static void RuchGracza(int _odKlienta, Pakiet _pakiet)
    {
        bool[] _wejścia = new bool[_pakiet.ReadInt()];
        for (int i = 0; i < _wejścia.Length; i++)
        {
            _wejścia[i] = _pakiet.ReadBool();
        }
        Quaternion _rotacja = _pakiet.CzytajKwaternion();

        Serwer.klienci[_odKlienta].gracz.UstawWejście(_wejścia, _rotacja);
    }
    //public static void OtrzymajTestSerwera(int _odKlienta, Pakiet _pakiet){
    //          string _msg = _pakiet.ReadString();
    //         Console.WriteLine($"Otrzymano pakiet po przez UDP. Jego wiadomość zawiera: {_msg}");
    //    }

}
