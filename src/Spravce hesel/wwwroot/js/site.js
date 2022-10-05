function PrepnoutBarevnyMotiv() {
    let cookies = document.cookie;

    if (cookies.length == 0)
        document.cookie = "Motiv=tmavy";
    else {
        document.cookie = "Motiv=;max-age=0";
    }

    window.location.reload();
}