import re

with open(r'G:\softwer\ERP.Hub\Components\Pages\EditEmployee.razor', 'r', encoding='utf-8') as f:
    orig = f.read()

def pascal_to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

# ============================================================
# PRE-PROCESS: MudProgressCircular outside of tabs
# ============================================================
orig = re.sub(
    r'<MudProgressCircular\s+Indeterminate="true"\s+Color="Color\.Primary"\s*/>',
    '<div class="flex justify-center items-center"><span class="spinner"></span></div>',
    orig
)

# ============================================================
# PARSE: Split file into: before_tabs + tabs_block + after_tabs
# ============================================================
m = re.search(
    r'(<MudTabs\s+@bind-ActivePanelIndex="(\w+)"[^>]*>)(.*?)(</MudTabs>)',
    orig, re.DOTALL
)
if not m:
    print("ERROR: Could not find MudTabs block")
    exit(1)

before = orig[:m.start()]
tab_var = m.group(2)
tabs_inner = m.group(3)
after = orig[m.end():]

# ============================================================
# EXTRACT each MudTabPanel
# ============================================================
panel_pat = re.compile(
    r'<MudTabPanel\s+Text="([^"]*)"\s+Icon="@Icons\.Material\.Filled\.(\w+)"[^>]*>\s*(.*?)\s*</MudTabPanel>',
    re.DOTALL
)

panels = []
for pm in panel_pat.finditer(tabs_inner):
    panels.append({
        'text': pm.group(1),
        'icon_name': pascal_to_snake(pm.group(2)),
        'content': pm.group(3)
    })
print(f"Found {len(panels)} MudTabPanels")

# ============================================================
# TRANSFORM: Strip MudXxx from panel content
# ============================================================
def transform(txt):
    # 0. Protect C# lambda => from being treated as HTML tag close
    txt = txt.replace('=>', '__LAMBDA__')

    # 1. Normalize Value="@("Male")" -> Value="Male"
    txt = re.sub(r'Value="@\("([^"]*)"\)"', r'Value="\1"', txt)

    # 2. MudImage
    txt = re.sub(r'<MudImage\s+([^>]*)/>', lambda m: _img(m.group(1)), txt, flags=re.DOTALL)

    # 3. MudText
    txt = re.sub(r'<MudText[^>]*>.*?</MudText>', lambda m: _text(m.group(0)), txt, flags=re.DOTALL)

    # 4. MudIcon
    txt = re.sub(r'<MudIcon\s+[^>]*/>', lambda m: _icon(m.group(0)), txt, flags=re.DOTALL)

    # 5. MudSelectItem (self-closing & with content)
    txt = re.sub(r'<MudSelectItem\s+Value="([^"]*)"\s*/>', lambda m: f'<option value="{m.group(1)}">{m.group(1)}</option>', txt)
    txt = re.sub(r'<MudSelectItem\s+Value="([^"]*)"[^>]*>(.*?)</MudSelectItem>', r'<option value="\1">\2</option>', txt, flags=re.DOTALL)

    # 6. MudSelect (with @bind-Value)
    txt = re.sub(
        r'<MudSelect\s+T="(\w+)"\s+@bind-Value="([^"]+)"([^>]*)>',
        lambda m: _sel_bind(m.group(2), m.group(3)),
        txt, flags=re.DOTALL
    )
    # MudSelect (Value/ValueChanged for string)
    def _sel_str(m):
        var = m.group(1)
        rest = m.group(2)
        hm = re.search(r'(\w+)\(', rest[rest.find('ValueChanged'):])
        handler = hm.group(1) if hm else 'handler'
        return f'<select class="form-control" value="@{var}" @onchange="@( (ChangeEventArgs e) => {{ {handler}(e.Value?.ToString() ?? string.Empty); }} )">'
    txt = re.sub(
        r'<MudSelect\s+T="string"\s+Value="(\w+)"([^>]*)>',
        _sel_str,
        txt, flags=re.DOTALL
    )
    # MudSelect (Value/ValueChanged for int with possible Disabled)
    def _sel_int(m):
        var = m.group(1)
        rest = m.group(2)
        hm = re.search(r'(\w+)\(', rest[rest.find('ValueChanged'):])
        handler = hm.group(1) if hm else 'handler'
        disabled = ''
        dm = re.search(r'Disabled="@([^"]*)"', rest)
        if dm: disabled = f' disabled="@{dm.group(1)}"'
        return f'<select class="form-control" value="@{var}" @onchange="@( (ChangeEventArgs e) => {{ int.TryParse(e.Value?.ToString() ?? "0", out var v); {handler}(v); }} )"{disabled}>'
    txt = re.sub(
        r'<MudSelect\s+T="int"\s+Value="([^"]+)"([^>]*)>',
        _sel_int,
        txt, flags=re.DOTALL
    )
    txt = txt.replace('</MudSelect>', '</select>')

    # 7. MudTextField (@bind-Value)
    def _tf_bind(m):
        bind = m.group(1)
        rest = m.group(2)
        pl = re.search(r'Placeholder="([^"]*)"', rest)
        ps = f' placeholder="{pl.group(1)}"' if pl else ''
        ml = re.search(r'MaxLength="([^"]*)"', rest)
        ms = f' maxlength="{ml.group(1)}"' if ml else ''
        return f'<input type="text" @bind-Value="{bind}" @bind-Value:event="oninput" class="form-control"{ps}{ms} />'
    txt = re.sub(r'<MudTextField\s+@bind-Value="([^"]+)"([^>]*)>', _tf_bind, txt, flags=re.DOTALL)

    # MudTextField (Value/ValueChanged)
    def _tf_val(m):
        var = m.group(1)
        rest = m.group(2)
        hm = re.search(r'(\w+)\(', rest[rest.find('ValueChanged'):])
        handler = hm.group(1) if hm else 'handler'
        pl = re.search(r'Placeholder="([^"]*)"', rest)
        ps = f' placeholder="{pl.group(1)}"' if pl else ''
        pl2 = re.search(r'Placeholder="@(\w+)"', rest)
        if pl2: ps = f' placeholder="@{pl2.group(1)}"'
        ml = re.search(r'MaxLength="([^"]*)"', rest)
        ms = f' maxlength="{ml.group(1)}"' if ml else ''
        err = ''
        em = re.search(r'Error="@(\w+)"', rest)
        if em: err += f' aria-invalid="@{em.group(1)}"'
        return f'<input type="text" value="@{var}" @onchange="@( (ChangeEventArgs e) => {handler}(e.Value?.ToString() ?? "") )" class="form-control"{ps}{ms}{err} />'
    txt = re.sub(r'<MudTextField\s+Value="(\w+)"([^>]*)>', _tf_val, txt, flags=re.DOTALL)
    txt = re.sub(r'</MudTextField>', '', txt)

    # 8. MudNumericField (@bind-Value)
    def _nf_bind(m):
        bind = m.group(1)
        rest = m.group(2)
        pl = re.search(r'Placeholder="([^"]*)"', rest)
        ps = f' placeholder="{pl.group(1)}"' if pl else ''
        return f'<input type="number" @bind-Value="{bind}" class="form-control"{ps} />'
    txt = re.sub(r'<MudNumericField\s+@bind-Value="([^"]+)"([^>]*)>', _nf_bind, txt, flags=re.DOTALL)

    # MudNumericField (Value with/without handler)
    def _nf_val(m):
        var = m.group(1)
        rest = m.group(2)
        disabled = ' disabled="true"' if 'Disabled="true"' in rest else ''
        pl = re.search(r'Placeholder="([^"]*)"', rest)
        ps = f' placeholder="{pl.group(1)}"' if pl else ''
        min_attr = ''
        mm = re.search(r'Min="(\d+)"', rest)
        if mm: min_attr = f' min="{mm.group(1)}"'
        hm = re.search(r'(\w+)\(', rest[rest.find('ValueChanged'):]) if 'ValueChanged' in rest else None
        handler = hm.group(1) if hm else None
        adorn = ''
        am = re.search(r'AdornmentText="([^"]*)"', rest)
        if am: adorn = am.group(1)
        input_html = f'<input type="number" value="@{var}" class="form-control"{ps}{min_attr}{disabled} />'
        if handler:
            input_html = f'<input type="number" value="@{var}" @onchange="@( (ChangeEventArgs e) => {{ decimal.TryParse(e.Value?.ToString() ?? "0", out var v); {handler}(v); }} )" class="form-control"{ps}{min_attr}{disabled} />'
        if adorn:
            return f'<span class="inline-flex items-center gap-1"><span class="text-zinc-500 text-sm">{adorn}</span>{input_html}</span>'
        return input_html
    txt = re.sub(r'<MudNumericField\s+Value="([^"]+)"([^>]*)>', _nf_val, txt, flags=re.DOTALL)
    txt = re.sub(r'</MudNumericField>', '', txt)

    # 9. MudDatePicker
    # Pattern 1: Date="_dob" DateChanged="OnDobChanged"
    txt = re.sub(
        r'<MudDatePicker\s+Date="(_dob)"[^>]*>',
        r'<input type="date" @bind-Value="\1" class="form-control" />',
        txt, flags=re.DOTALL
    )
    # Pattern 2: Date="@_joiningDate" DateChanged="@( (DateTime? d) => OnJoiningDateChanged(d) )"
    def _dp_join(m):
        rest = m.group(1)
        hm = re.search(r'(\w+)\(', rest[rest.find('DateChanged'):])
        handler = hm.group(1) if hm else 'handler'
        return f'<input type="date" value="@_joiningDate" @onchange="@( (ChangeEventArgs e) => {{ DateTime.TryParse(e.Value?.ToString() ?? "", out var d); {handler}(d); }} )" class="form-control" />'
    txt = re.sub(
        r'<MudDatePicker\s+Date="@_joiningDate"([^>]*)>',
        _dp_join,
        txt, flags=re.DOTALL
    )
    txt = re.sub(r'<MudDatePicker[^>]*>', '<input type="date" class="form-control" />', txt, flags=re.DOTALL)
    txt = re.sub(r'</MudDatePicker>', '', txt)

    # 10. MudSwitch
    def _sw(m):
        bind = m.group(1)
        rest = m.group(2)
        cm = re.search(r'Class="([^"]*)"', rest)
        cls = f' {cm.group(1)}' if cm else ''
        lm = re.search(r'Label="([^"]*)"', rest)
        label = lm.group(1) if lm else ''
        return (f'<label class="switch{cls}">\n'
                f'        <input type="checkbox" @bind-Value="{bind}" />\n'
                f'        <span class="switch-track"><span class="switch-thumb"></span></span>\n'
                f'        {label}\n'
                f'    </label>')
    txt = re.sub(r'<MudSwitch\s+@bind-Value="([^"]+)"([^>]*)/>', _sw, txt, flags=re.DOTALL)

    # 11. MudCheckBox
    def _cb(m):
        var = m.group(1)
        rest = m.group(2)
        lm = re.search(r'Label="([^"]*)"', rest)
        label = lm.group(1) if lm else ''
        hm = re.search(r'(\w+)\(', rest[rest.find('ValueChanged'):])
        handler = hm.group(1) if hm else 'handler'
        return (f'<label class="checkbox-container">\n'
                f'        <input type="checkbox" checked="@{var}" @onchange="@( (ChangeEventArgs e) => {handler}(e.Value?.ToString() == "true") )" class="checkbox-input" />\n'
                f'        <span class="checkbox-checkmark"></span>\n'
                f'        <span class="checkbox-label">{label}</span>\n'
                f'    </label>')
    txt = re.sub(r'<MudCheckBox\s+T="bool"\s+Value="(\w+)"([^>]*)>', _cb, txt, flags=re.DOTALL)

    # 12. MudItem
    def _item(m):
        a = m.group(1)
        cls = ''
        cm = re.search(r'Class="([^"]*)"', a)
        if cm: cls = ' ' + cm.group(1)
        xs = re.search(r'xs="(\w+)"', a)
        sm = re.search(r'sm="(\w+)"', a)
        lg = re.search(r'lg="(\w+)"', a)
        xs_v = xs.group(1) if xs else ''
        sm_v = sm.group(1) if sm else ''
        lg_v = lg.group(1) if lg else ''
        if xs_v == '12' and sm_v == '6':
            cl = ['col-span-1', 'sm:col-span-2']
        elif xs_v == '12' and lg_v == '6':
            cl = ['col-span-1', 'lg:col-span-2']
        else:
            cl = ['col-span-1']
        return f'<div class="{" ".join(cl)}{cls}">'
    txt = re.sub(r'<MudItem\s+([^>]*)>', _item, txt, flags=re.DOTALL)
    txt = txt.replace('</MudItem>', '</div>')

    # 13. MudGrid
    txt = txt.replace('<MudGrid>', '<div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">')
    txt = txt.replace('</MudGrid>', '</div>')

    # Restore lambda expressions
    txt = txt.replace('__LAMBDA__', '=>')
    return txt

# Helper functions
def _img(a):
    src = alt = cls = ''
    w = h = of = ''
    for k, v in re.findall(r'(\w+)="([^"]*)"', a):
        if k == 'Src': src = v
        elif k == 'Alt': alt = v
        elif k == 'Class': cls = v
        elif k == 'Width': w = v
        elif k == 'Height': h = v
        elif k == 'ObjectFit': of = v.replace('ObjectFit.', '').lower()
    sa = []
    if w: sa.append(f'width:{w}px')
    if h: sa.append(f'height:{h}px')
    if of: sa.append(f'object-fit:{of}')
    ss = f' style="{";".join(sa)}"' if sa else ''
    cs = f' class="{cls}"' if cls else ''
    return f'<img src="{src}" alt="{alt}"{cs}{ss} />'

def _text(f):
    tag = 'p'
    tm = re.search(r'Typo="Typo\.(\w+)"', f)
    if tm:
        t = tm.group(1)
        if t in ('body2', 'body1'): tag = 'p'
        elif t in ('h6', 'subtitle1', 'subtitle2'): tag = 'h6'
        elif t == 'h5': tag = 'h5'
        elif t == 'caption': tag = 'span'
    cm = re.search(r'Class="([^"]*)"', f)
    cls = cm.group(1) if cm else ''
    inner = re.sub(r'<MudText[^>]*>', '', f)
    inner = re.sub(r'</MudText>', '', inner)
    cs = f' class="{cls}"' if cls else ''
    return f'<{tag}{cs}>{inner}</{tag}>'

def _icon(f):
    icon = ''
    fs = '20px'
    cls = ''
    im = re.search(r'Icon="@Icons\.Material\.Filled\.(\w+)"', f)
    if im: icon = pascal_to_snake(im.group(1))
    sm = re.search(r'Size="Size\.(\w+)"', f)
    if sm: fs = {'small': '16px', 'medium': '20px', 'large': '32px'}.get(sm.group(1), '20px')
    cm = re.search(r'Class="([^"]*)"', f)
    if cm: cls = ' ' + cm.group(1)
    return f'<span class="material-icons{cls}" style="font-size:{fs};">{icon}</span>'

def _sel_bind(bind_val, rest):
    disabled = ''
    dm = re.search(r'Disabled="([^"]*)"', rest)
    if dm: disabled = f' disabled="{dm.group(1)}"'
    return f'<select class="form-control" @bind-Value="{bind_val}"{disabled}>'

# Transform each panel
INDENT = '                                    '  # 36 spaces for content inside tab-panel
for i, p in enumerate(panels):
    raw = p['content']
    transformed = transform(raw)
    lines = transformed.split('\n')
    new_lines = []
    for line in lines:
        if line.strip():
            new_lines.append(INDENT + line.strip())
        else:
            new_lines.append('')
    p['content'] = '\n'.join(new_lines)

# ============================================================
# REASSEMBLE
# ============================================================
tab_buttons = []
tab_panels_html = []

for i, p in enumerate(panels):
    btn_indent = '                            '
    btn = (f'{btn_indent}<button class="tab @({tab_var} == {i} ? "active" : "")" @onclick="() => {tab_var} = {i}">\n'
           f'{btn_indent}    <span class="material-icons" style="font-size:16px;">{p["icon_name"]}</span> {p["text"]}\n'
           f'{btn_indent}</button>')
    tab_buttons.append(btn)

    pnl_indent = '                            '
    pnl = (f'{pnl_indent}<div class="tab-panel @({tab_var} == {i} ? "active" : "")">\n'
           f'{p["content"]}\n'
           f'{pnl_indent}</div>')
    tab_panels_html.append(pnl)

new_tabs = (
    '                        <div class="tabs add-employee-tabs">\n'
    '                            <div class="tabs-header">\n'
    + '\n'.join(tab_buttons) + '\n'
    '                            </div>\n'
    '                            <div class="tabs-content">\n'
    + '\n'.join(tab_panels_html) + '\n'
    '                            </div>\n'
    '                        </div>'
)

# Strip trailing whitespace from before (it already includes leading indent from original MudTabs)
before = before.rstrip()
result = before + '\n' + new_tabs + '\n' + after

with open(r'G:\softwer\ERP.Hub\Components\Pages\EditEmployee.razor', 'w', encoding='utf-8') as f:
    f.write(result)

remaining = [l for l in result.split('\n') if re.search(r'</?Mud[A-Z]', l)]
print(f"Done. {len(result)} chars, {len(result.split(chr(10)))} lines")
print(f"Remaining MudXxx: {len(remaining)}")
for r in remaining:
    print(f"  Mud: {r.strip()[:120]}")
