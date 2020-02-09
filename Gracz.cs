using System.Collections;
using System.Collections.Generic;
using UnityEngine;
class Gracz : MonoBehaviour
{
    public int id;
    public string nazwaGracza;

    private float szybkośćPoruszania = 5f / Stałe.TIKI_NA_SEKUNDĘ;
    private bool[] wejścia;

    public void Inicjalizuj(int _id, string _nazwaGracza)
    {
        id = _id;
        nazwaGracza = _nazwaGracza;

        wejścia = new bool[6];

    }
    public void FixedUpdate()
    {
        Vector2 _kierunekWejścia = Vector2.zero;
        Quaternion _kierunekRotacji = Quaternion.Euler(0,0,0);
        if (wejścia[0]) { _kierunekWejścia.y += 1; }
        if (wejścia[1]) { _kierunekWejścia.y -= 1; }
        if (wejścia[2]) { _kierunekWejścia.x -= 1; }
        if (wejścia[3]) { _kierunekWejścia.x += 1; }

        if (wejścia[4]) { _kierunekRotacji.y -= 1.0f; }
        if (wejścia[5]) { _kierunekRotacji.y += 1.1f; }


        Ruch(_kierunekWejścia, _kierunekRotacji);
    }
    private void Ruch(Vector2 _kierunekWejścia, Quaternion _kierunekRotacji)
    {

        Vector3 _kierunekRuchu = transform.right * _kierunekWejścia.x + transform.forward * _kierunekWejścia.y;
        transform.position += _kierunekRuchu * szybkośćPoruszania;
        WysyłkaSerwera.PozycjaGracza(this);

        Quaternion nowaRotacja = transform.rotation * Quaternion.Euler(0, _kierunekRotacji.y, 0);
        transform.rotation = nowaRotacja;

        WysyłkaSerwera.RotacjaGracza(this);
    }

    public void UstawWejście(bool[] _wejścia, Quaternion _rotacja)
    {
        wejścia = _wejścia;
        transform.rotation = _rotacja;
    }

}
