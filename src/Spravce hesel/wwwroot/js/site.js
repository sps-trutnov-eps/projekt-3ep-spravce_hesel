function PrepnoutBarevnyMotiv() {
    if (!document.cookie.includes("TmavyMotiv"))
        document.cookie = "TmavyMotiv=true; path=/; expires=Fri, 31 Dec 9999 23:59:59 GMT";
    else
        document.cookie = "TmavyMotiv=; path=/; max-age=0";

    window.location.reload();
    }

function vyzadatHeslo(id) {
    $.ajax({
        type: "GET",
        url: "/Hesla/DetailHesla/" + id,
        dataType: "JSON",
        data: JSON.stringify(id),
        contentType: "application/json; charset=utf-8",
        
        success: (res) => {
            zobrazitPOPup();
            document.getElementById("sluzba").innerHTML = res.value.sluzba;
            document.getElementById("jmeno").innerHTML = res.value.jmeno;
            document.getElementById("heslo").innerHTML = res.value.sifra;
        }
    });
}

function zobrazitPOPup() {
    document.getElementsByClassName("POPup")[0].className = "POPup";
    document.getElementsByClassName("POPupPozadi")[0].className = "POPupPozadi";
}

function skrytPOPUp() {
    document.getElementsByClassName("POPup")[0].className = "POPup skryty";
    document.getElementsByClassName("POPupPozadi")[0].className = "POPupPozadi skryty";
}