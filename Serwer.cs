using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class Serwer
{
    public static int MaksGraczy { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Klient> klienci = new Dictionary<int, Klient>();

    public delegate void ObsługaPakietu(int _odKlienta, Pakiet _pakiet);


    public static Dictionary<int, ObsługaPakietu> obsługaPakietu;

    private static TcpListener tcpSłuchacz;
    private static UdpClient nasłuchujUdp;
    public static void Start(int _maksGraczy, int _port)
    {
        MaksGraczy = _maksGraczy;
        Port = _port;

        Console.WriteLine("Uruchamianie serwera... ");
        InicjalizujDaneSerwera();


        tcpSłuchacz = new TcpListener(IPAddress.Any, Port);
        tcpSłuchacz.Start();
        tcpSłuchacz.BeginAcceptTcpClient(new AsyncCallback(TCPConnectionCallback), null);

        nasłuchujUdp = new UdpClient(Port);
        nasłuchujUdp.BeginReceive(UDPOtrzymajOddzwoń, null);


        Console.WriteLine($"Serwer uruchomiony na porcie {Port}.");

    }

    private static void TCPConnectionCallback(IAsyncResult _result)
    {
        TcpClient _klient = tcpSłuchacz.EndAcceptTcpClient(_result);
        tcpSłuchacz.BeginAcceptTcpClient(new AsyncCallback(TCPConnectionCallback), null);
        Console.WriteLine($"Nadchodzące połączenie od {_klient.Client.RemoteEndPoint}");

        for (int i = 1; i <= MaksGraczy; i++)
        {
            if (klienci[i].tcp.gniazdo == null)
            {
                klienci[i].tcp.Połącz(_klient);
                return;
            }
        }

        Console.WriteLine($"{_klient.Client.RemoteEndPoint} nie udało się połączyć: Serwer jest Pełen!");

    }

    private static void UDPOtrzymajOddzwoń(IAsyncResult _rezultat)
    {
        try
        {
            IPEndPoint _końcowyPunktKlienta = new IPEndPoint(IPAddress.Any, 0);
            byte[] _dane = nasłuchujUdp.EndReceive(_rezultat, ref _końcowyPunktKlienta);
            nasłuchujUdp.BeginReceive(UDPOtrzymajOddzwoń, null);

            if (_dane.Length < 4)
            {
                return;
            }

            using (Pakiet _pakiet = new Pakiet(_dane))
            {
                int _idKlienta = _pakiet.ReadInt();

                if (_idKlienta == 0)
                {

                    return;
                }
                if (klienci[_idKlienta].udp.punktKońcowy == null)
                {
                    klienci[_idKlienta].udp.Połącz(_końcowyPunktKlienta);
                    return;
                }

                if (klienci[_idKlienta].udp.punktKońcowy.ToString() == _końcowyPunktKlienta.ToString())
                {
                    klienci[_idKlienta].udp.ObsłużDane(_pakiet);

                }
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Błąd w otrzymaniu danych UDP: {_ex}");
        }

    }

    public static void WyślijDaneUDP(IPEndPoint _końcowyPunktKlienta, Pakiet _pakiet)
    {
        try
        {
            if (_końcowyPunktKlienta != null)
            {
                nasłuchujUdp.BeginSend(_pakiet.ToArray(), _pakiet.Length(), _końcowyPunktKlienta, null, null);
            }
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Błąd przesyłu danych do {_końcowyPunktKlienta} po przez UDP: {_ex}");
        }

    }


    private static void InicjalizujDaneSerwera()
    {

        for (int i = 1; i <= MaksGraczy; i++)
        {
            klienci.Add(i, new Klient(i));

        }

        obsługaPakietu = new Dictionary<int, ObsługaPakietu>()
            {

                { (int)PakietyKlienta.powitanieOtrzymane, ObsługaSerwera.PowitanieOtrzymane },
                { (int)PakietyKlienta.RuchGracza, ObsługaSerwera.RuchGracza },
               // { (int)PakietyKlienta.udpTestOtrzymania, ObsługaSerwera.OtrzymajTestSerwera }
            };

        Console.WriteLine("Zainicjalizowano pakiety.");

    }
}
