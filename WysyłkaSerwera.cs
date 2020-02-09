using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class WysyłkaSerwera : MonoBehaviour
{
    private static void SendTCPData(int _doKlienta, Pakiet _pakiet)
    {
        _pakiet.WriteLength();
        Serwer.klienci[_doKlienta].tcp.WyślijDane(_pakiet);
    }
    private static void WyślijDaneUDP(int _doKlienta, Pakiet _pakiet)
    {
        _pakiet.WriteLength();
        Serwer.klienci[_doKlienta].udp.WyślijDane(_pakiet);
    }
    private static void WyślijDaneTCPdoWszystkich(Pakiet _pakiet)
    {
        _pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            Serwer.klienci[i].tcp.WyślijDane(_pakiet);
        }
    }
    private static void WyślijDaneTCPdoWszystkich(int _wyłączKlienta, Pakiet _pakiet)
    {
        _pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            if (i != _wyłączKlienta)
            {
                Serwer.klienci[i].tcp.WyślijDane(_pakiet);

            }
        }
    }
    private static void WyślijDaneUDPdoWszystkich(Pakiet _pakiet)
    {
        _pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            Serwer.klienci[i].udp.WyślijDane(_pakiet);
        }

    }
    private static void WyślijDaneUDPdoWszystkich(int _wyłączKlienta, Pakiet _pakiet)
    {
        _pakiet.WriteLength();
        for (int i = 1; i <= Serwer.MaksGraczy; i++)
        {
            if (i != _wyłączKlienta)
            {
                Serwer.klienci[i].udp.WyślijDane(_pakiet);

            }

        }
    }
    #region Pakiety
    public static void Powitanie(int _doKlienta, string _msg)
    {
        using (Pakiet _pakiet = new Pakiet((int)PakietySerwera.welcome))
        {
            _pakiet.Write(_msg);
            _pakiet.Write(_doKlienta);

            SendTCPData(_doKlienta, _pakiet);
        }
    }

    public static void OdródźGracza(int _doKlienta, Gracz _gracz)
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

         //   WyślijDaneUDPdoWszystkich(_gracz.id, _pakiet);
            WyślijDaneUDPdoWszystkich( _pakiet);

        }
    }

    //    public static void UDPTest(int _doKlienta) {
    //       using (Pakiet _pakiet = new Pakiet((int)PakietySerwera.udpTest)) {
    //         _pakiet.Write("Pakiet testowy dla UDP.");        
    //         WyślijDaneUDP(_doKlienta, _pakiet);
    //      }
    // }


    #endregion
}
