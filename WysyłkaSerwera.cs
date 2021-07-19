using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class WysyłkaSerwera : MonoBehaviour
{
    private static void SendTCPData(int doKlienta, Pakiet pakiet)
    {
        pakiet.WriteLength();
        Serwer.klienci[doKlienta].tcp.WyślijDane(pakiet);
    }
    private static void WyślijDaneUDP(int doKlienta, Pakiet pakiet)
    {
        _pakiet.WriteLength();
        Serwer.klienci[_doKlienta].udp.WyślijDane(_pakiet);
    }
    private static void WyślijDaneTCPdoWszystkich(Pakiet pakiet)
    {
        pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            Serwer.klienci[i].tcp.WyślijDane(pakiet);
        }
    }
    private static void WyślijDaneTCPdoWszystkich(int wyłączKlienta, Pakiet pakiet)
    {
        pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            if (i != _wyłączKlienta)
            {
                Serwer.klienci[i].tcp.WyślijDane(pakiet);
            }
        }
    }
    private static void WyślijDaneUDPdoWszystkich(Pakiet pakiet)
    {
        pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            Serwer.klienci[i].udp.WyślijDane(pakiet);
        }

    }
    private static void WyślijDaneUDPdoWszystkich(int wyłączKlienta, Pakiet pakiet)
    {
        pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            if (i != _wyłączKlienta)
            {
                Serwer.klienci[i].udp.WyślijDane(_pakiet);
            }
        }
    }

    #region Pakiety

    public static void Powitanie(int doKlienta, string msg)
    {
        using (Pakiet _pakiet = new Pakiet((int)PakietySerwera.welcome))
        {
            _pakiet.Write(_msg);
            _pakiet.Write(_doKlienta);
            SendTCPData(_doKlienta, _pakiet);
        }
    }
    public static void OdródźGracza(int doKlienta, Gracz _gracz)
    {
        using (Pakiet _pakiet = new Pakiet((int)PakietySerwera.odródźGracza))
        {
            _pakiet.Write(_gracz.id);
            _pakiet.Write(_gracz.nazwaGracza);
            _pakiet.Zapisz(_gracz.transform.position);
            _pakiet.Zapisz(_gracz.transform.rotation);
            SendTCPData(_doKlienta, _pakiet);
        }
    }
    public static void PozycjaGracza(Gracz _gracz)
    {
        using (Pakiet _pakiet = new Pakiet((int)PakietySerwera.pozycjaGracza))
        {
            _pakiet.Write(_gracz.id);
            _pakiet.Zapisz(_gracz.transform.position);
            WyślijDaneUDPdoWszystkich(_pakiet);
        }
    }
    public static void RotacjaGracza(Gracz _gracz)
    {
        using (Pakiet _pakiet = new Pakiet((int)PakietySerwera.rotacjaGracza))
        {
            _pakiet.Write(_gracz.id);
            _pakiet.Zapisz(_gracz.transform.rotation);
            WyślijDaneUDPdoWszystkich( _pakiet);
        }
    }
    #endregion

}
