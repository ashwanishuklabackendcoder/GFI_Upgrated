(function () {
    document.addEventListener('click', function (e) {

        const toggle = e.target.closest('.menu-toggle');
        if (!toggle) return;

        const menu = document.getElementById('layout-menu');
        const item = toggle.closest('.menu-item');

        if (!menu || !item) return;

        const isHorizontal = menu.classList.contains('menu-horizontal');
        const isRoot = item.parentElement?.classList.contains('menu-inner');

        setTimeout(() => {

            if (isHorizontal && isRoot && item.classList.contains('open')) {

                document.querySelectorAll('.menu-inner > .menu-item.open')
                    .forEach(openItem => {
                        if (openItem !== item) {
                            openItem.classList.remove('open');
                        }
                    });
            }

        }, 100); // slightly increased delay

    });
})();