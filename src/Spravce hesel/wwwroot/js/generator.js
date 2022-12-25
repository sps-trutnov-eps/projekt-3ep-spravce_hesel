function NahodneCislo(max) {
    return Math.floor(Math.random() * max);
}

function zobrazeniHesla() {
    let heslo = document.getElementById("gen_vystup");
    let tlacitko = document.getElementById("zobrazeni_hesla_tlacitko");

    if (tlacitko.className == "icon-eye zobrazeni_Hesla") {
        heslo.type = "text";
        tlacitko.className = "icon-eye-off zobrazeni_Hesla";
    }
    else {
        heslo.type = "password";
        tlacitko.className = "icon-eye zobrazeni_Hesla";
    };
}

function delkaHeslaAktualizace() {
    document.getElementById("posuvnikVystup").value = document.getElementById("gen_delka").value;
}

function delkaHeslaZapis() {
    document.getElementById("gen_delka").value = document.getElementById("posuvnikVystup").value;
}

function generace() {
    let form = document.getElementById("generator");
    const skupinySymbolu = [
        ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"],
        ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"],
        ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"],
        [",", ".", "_", "-", "!", "^", "°", ";", "?", "$", "€", "<", ">", "*", "/", "@"]
    ]
    let pouzivaneSymboly = [];
    let vysledneHeslo = "";
    const vyber = [
        form.gen_mala.checked,
        form.gen_velka.checked,
        form.gen_cisla.checked,
        form.gen_znaky.checked
    ]
    let count = 0;

    vyber.forEach(skupina => {
        if (skupina)
            pouzivaneSymboly.push(skupinySymbolu[count]);
        count += 1;
    });

    let skupinaSymbolu;
    count = 0;
    while (count != form.gen_delka.value) {
        skupinaSymbolu = pouzivaneSymboly[NahodneCislo(pouzivaneSymboly.length)];
        vysledneHeslo = vysledneHeslo.concat(skupinaSymbolu[NahodneCislo(skupinaSymbolu.length)]);
        count += 1;
    };

    document.getElementById("gen_vystup").value = vysledneHeslo;
}

$(document).ready(() => {
    $(".zobrazeni_Hesla").click(zobrazeniHesla);
    $(".generace").click(generace);
    $("#generator").change(delkaHeslaAktualizace);
});
