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
            zobrazitPOPup("detaily");
            document.getElementById("sluzba").innerHTML = res.value.sluzba;
            document.getElementById("jmeno").innerHTML = res.value.jmeno;
            document.getElementById("heslo").innerHTML = res.value.sifra;
        }
    });
}

function vyzadatOdstraneni(id) {
    document.getElementById("odstraneni").action = "/Hesla/Odstranit/" + id;
    zobrazitPOPup("odstranit");
}

function zobrazitPOPup(id) {
    skrytPOPUp();
    document.getElementsByClassName("POPup")[id].className = "POPup";
    document.getElementsByClassName("POPupPozadi")[0].className = "POPupPozadi";
}

function skrytPOPUp() {
    const POPupy = document.getElementsByClassName("POPup");
    const POPupPozadi = document.getElementsByClassName("POPupPozadi");

    for (let div of POPupy) {
        div.className = "POPup skryty";
    };

    for (let div of POPupPozadi) {
        div.className = "POPupPozadi skryty";
    };
}

