(() => {
    const content = document.getElementById('content');
    if (content.dataset.mode === 'sync') return;
    let pending;
    function load(url) {
        if (pending) pending.abort();
        content.setAttribute('aria-busy', 'true');
        pending = $.ajax({url: content.dataset.endpoint, data: url.searchParams.toString(), dataType: 'html'})
            .done(html => {
                $(content).html(html);
                history.replaceState(null, '', url);
                const mid = url.searchParams.get('mid') || '';
                document.querySelectorAll('[data-major]').forEach(link => {
                    link.classList.toggle('active', link.dataset.major === mid);
                    const next = new URL(url);
                    next.searchParams.delete('pageIndex');
                    if (link.dataset.major) next.searchParams.set('mid', link.dataset.major);
                    else next.searchParams.delete('mid');
                    link.href = next.href;
                });
                const field = document.querySelector('#search-form input[name="mid"]');
                if (field) field.value = mid;
                document.querySelectorAll('a[href*="mode="]:not([data-major])').forEach(link => {
                    const next = new URL(url);
                    next.searchParams.set('mode', new URL(link.href).searchParams.get('mode'));
                    link.href = next.href;
                });
            })
            .fail((xhr, status) => { if (status !== 'abort') window.location.assign(url); })
            .always(() => content.removeAttribute('aria-busy'));
    }
    $(document).on('click', '#major-list [data-major], #content [data-page]', function(e) {
        if (e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
        e.preventDefault();
        load(new URL(this.href));
    });
    $('#search-form').on('submit', function(e) {
        e.preventDefault();
        const url = new URL(this.action);
        for (const [name, value] of new FormData(this)) {
            if (value.trim()) url.searchParams.set(name, value.trim());
        }
        load(url);
    });
})();
