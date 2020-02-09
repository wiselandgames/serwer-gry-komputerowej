using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

class Klient
{

    public static int rozmiarBufferaDanych = 4096;

    public int id;
    public Gracz gracz;
    public TCP tcp;
    public UDP udp;


    public Klient(int _klientId)
    {
        id = _klientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }


    public class TCP
    {

        public TcpClient gniazdo;

        private readonly int id;
        private NetworkStream strumień;
        private Pakiet otrzymaneDane;

        private byte[] otrzymajBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        public void Połącz(TcpClient _gniazdo)
        {
            gniazdo = _gniazdo;
            gniazdo.ReceiveBufferSize = rozmiarBufferaDanych;
            gniazdo.SendBufferSize = rozmiarBufferaDanych;

            strumień = gniazdo.GetStream();
            otrzymaneDane = new Pakiet();
            otrzymajBuffer = new byte[rozmiarBufferaDanych];
            strumień.BeginRead(otrzymajBuffer, 0, rozmiarBufferaDanych, OtrzymajOddzwonienie, null);
            WysyłkaSerwera.Powitanie(id, "Witaj na serwerze!");
        }

        public void WyślijDane(Pakiet _pakiet)
        {
            try
            {
                if (gniazdo != null)
                {
                    strumień.BeginWrite(_pakiet.ToArray(), 0, _pakiet.Length(), null, null);

                }
            }
            catch
            {
                Debug.Log($"Błąd przesyłu danych do gracza {id} via TCP");
            }


        }

        private void OtrzymajOddzwonienie(IAsyncResult _wynik)
        {
            try
            {
                int _długośćBitów = strumień.EndRead(_wynik);
                if (_długośćBitów <= 0)
                {

                    Serwer.klienci[id].Rozłącz();
                    return;
                }

                byte[] _dane = new byte[_długośćBitów];
                Array.Copy(otrzymajBuffer, _dane, _długośćBitów);

                otrzymaneDane.Reset(ObsłużDane(_dane));
                strumień.BeginRead(otrzymajBuffer, 0, rozmiarBufferaDanych, OtrzymajOddzwonienie, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Błąd w otrzymania danych TCP: {_ex}");
                Serwer.klienci[id].Rozłącz();

            }

        }

        private bool ObsłużDane(byte[] _dane)
        {
            int _długośćPakietu = 0;

            otrzymaneDane.SetBytes(_dane);

            if (otrzymaneDane.UnreadLength() >= 4)
            {

                _długośćPakietu = otrzymaneDane.ReadInt();
                if (_długośćPakietu <= 0)
                {
                    return true;
                }
            }

            while (_długośćPakietu > 0 && _długośćPakietu <= otrzymaneDane.UnreadLength())
            {
                byte[] _bityPakietu = otrzymaneDane.ReadBytes(_długośćPakietu);
                ManagerWątku.ExecuteOnMainThread(() =>
                {

                    using (Pakiet _pakiet = new Pakiet(_bityPakietu))
                    {
                        int _idPakietu = _pakiet.ReadInt();

                        Serwer.obsługaPakietu[_idPakietu](id, _pakiet);
                    }

                });


                _długośćPakietu = 0;


                if (otrzymaneDane.UnreadLength() >= 4)
                {

                    _długośćPakietu = otrzymaneDane.ReadInt();
                    if (_długośćPakietu <= 0)
                    {
                        return true;
                    }
                }

            }

            if (_długośćPakietu <= 1)
            {

                return true;
            }
            return false;
        }

        public void Rozłącz()
        {
            gniazdo.Close();
            strumień = null;
            otrzymaneDane = null;
            otrzymajBuffer = null;
            gniazdo = null;

        }

    }

    public class UDP
    {
        public IPEndPoint punktKońcowy;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        public void Połącz(IPEndPoint _punktKońcowy)
        {
            punktKońcowy = _punktKońcowy;
            //SerwerWyślij.UDPTest(id);
        }
        public void WyślijDane(Pakiet _pakiet)
        {
            Serwer.WyślijDaneUDP(punktKońcowy, _pakiet);

        }
        public void ObsłużDane(Pakiet _danePakietu)
        {
            int _długośćPakietu = _danePakietu.ReadInt();
            byte[] _bityPakietu = _danePakietu.ReadBytes(_długośćPakietu);

            ManagerWątku.ExecuteOnMainThread(() =>
            {
                using (Pakiet _pakiet = new Pakiet(_bityPakietu))
                {

                    int _idPakietu = _pakiet.ReadInt();
                    Serwer.obsługaPakietu[_idPakietu](id, _pakiet);
                }
            });
        }

        public void Rozłącz()
        {
            punktKońcowy = null;
        }
    }

    public void WyślijDoGry(string _nazwaGracza)
    {
        gracz = ManagerSieci.instancja.InstancjujGracza();
        gracz.Inicjalizuj(id, _nazwaGracza);

        foreach (Klient _klient in Serwer.klienci.Values)
        {
            if (_klient.gracz != null)
            {
                if (_klient.id != id)
                {
                    WysyłkaSerwera.OdródźGracza(id, _klient.gracz);
                }
            }
        }
        foreach (Klient _klient in Serwer.klienci.Values)
        {
            if (_klient.gracz != null)
            {
                WysyłkaSerwera.OdródźGracza(_klient.id, gracz);
            }

        }

    }

    private void Rozłącz()
    {
        Debug.Log($"{tcp.gniazdo.Client.RemoteEndPoint} został rozłączony.");

        UnityEngine.Object.Destroy(gracz.gameObject);
        gracz = null;

        tcp.Rozłącz();
        udp.Rozłącz();

    }


}
