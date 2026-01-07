using System;
using System.Collections.Generic;
using System.Text;

namespace Nesdesign.Models
{
        public enum OfferStatus
        {
            [System.ComponentModel.Description("Utworzona")]
            UTWORZONA,
            [System.ComponentModel.Description("Nie ofertowana")]
            NIE_OFERTOWANA,
            [System.ComponentModel.Description("Oferta mailowa")]
            OFERTA_MAILOWA,
            [System.ComponentModel.Description("Oferta")]
            OFERTA,
            [System.ComponentModel.Description("Zamówienie")]
            ZAMOWIENIE,
            [System.ComponentModel.Description("W realizacji")]
            W_REALIZACJI,
            [System.ComponentModel.Description("Gotowe")]
            GOTOWA,
            [System.ComponentModel.Description("Wysłane")]
            WYSLANA,
            [System.ComponentModel.Description("Zakończone")]
            ZAKONCZONA,

            [System.ComponentModel.Description("Zamówienie częściowe")]
            CZESCIOWE_ZAMOWIENIE,
            [System.ComponentModel.Description("Reklamacja")]
            REKLAMACJA,
            [System.ComponentModel.Description("Anulowane")]
            ANULOWANE,
            [System.ComponentModel.Description("Wstrzymane")]
            WSTRZYMANE,
            [System.ComponentModel.Description("W produkcji")]
            W_PRODUKCJI,
            [System.ComponentModel.Description("Rozliczone")]
            ROZLICZONE

        }
    
}
