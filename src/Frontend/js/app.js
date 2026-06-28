// Config de la API
var API_BASE = 'https://perfeck-rankings-api.azurewebsites.net';

// Al cargar la pagina
document.addEventListener('DOMContentLoaded', function() {
    verificarAPI();
    cargarRankings();
});

function verificarAPI() {
    var el = document.getElementById('api-status');
    fetch(API_BASE + '/health')
        .then(function(r) { return r.json(); })
        .then(function(data) {
            el.innerHTML = 'API: ' + data.status + ' - ' + new Date(data.timestamp).toLocaleString();
            el.className = 'api-ok';
        })
        .catch(function() {
            el.innerHTML = 'API: Sin conexion';
            el.className = 'api-error';
        });
}

// Cambio de pestañas
function mostrarTab(tab) {
    var tabs = ['rankings', 'torneos', 'jugadores', 'stats'];
    for (var i = 0; i < tabs.length; i++) {
        document.getElementById('tab-' + tabs[i]).style.display = 'none';
    }
    document.getElementById('tab-' + tab).style.display = 'block';
    
    var btns = document.querySelectorAll('.nav-bar button');
    for (var j = 0; j < btns.length; j++) { btns[j].className = ''; }
    event.target.className = 'activo';
    
    if (tab === 'rankings') cargarRankings();
    if (tab === 'torneos') cargarTorneos();
    if (tab === 'jugadores') cargarJugadores();
    if (tab === 'stats') cargarStats();
}

// RANKINGS
function cargarRankings() {
    var el = document.getElementById('rankings-content');
    fetch(API_BASE + '/api/rankings')
        .then(function(r) { return r.json(); })
        .then(function(data) {
            var html = '<div class="tabla-wrapper"><table><thead><tr>';
            html += '<th>#</th><th>Jugador</th><th>ELO</th><th>Torneos</th><th>Victorias</th>';
            html += '</tr></thead><tbody>';
            
            for (var i = 0; i < data.length; i++) {
                var j = data[i];
                var posClass = i === 0 ? 'pos-1' : i === 1 ? 'pos-2' : i === 2 ? 'pos-3' : '';
                var avatar = j.avatar || 'https://ui-avatars.com/api/?name=' + j.nombre + '&background=000000&color=00ffff&size=30';
                html += '<tr>';
                html += '<td class="' + posClass + '">' + (i + 1) + '</td>';
                html += '<td><div class="jugador-celda"><img src="' + avatar + '" class="avatar-img" alt=""><span>' + j.nombre + '</span></div></td>';
                html += '<td><span class="elo-badge">' + j.elo + '</span></td>';
                html += '<td>' + j.totalTorneos + '</td>';
                html += '<td>' + j.victorias + '</td>';
                html += '</tr>';
            }
            
            html += '</tbody></table></div>';
            el.innerHTML = html;
        })
        .catch(function() {
            el.innerHTML = '<p style="color:#FF5050;">Error al cargar rankings</p>';
        });
}

// TORNEOS
function cargarTorneos() {
    var el = document.getElementById('torneos-content');
    document.getElementById('torneo-detalle').style.display = 'none';
    
    fetch(API_BASE + '/api/torneos')
        .then(function(r) { return r.json(); })
        .then(function(data) {
            var html = '<div class="cuadros-grid">';
            
            for (var i = 0; i < data.length; i++) {
                var t = data[i];
                var badgeClass = t.estado === 'EnCurso' ? 'badge-activo' : 
                                t.estado === 'Inscripcion' ? 'badge-inscripcion' : 'badge-finalizado';
                html += '<div class="cuadro" onclick="verTorneo(' + t.id + ')">';
                html += '<h3>' + t.nombre + '</h3>';
                html += '<span class="badge ' + badgeClass + '">' + t.estado + '</span>';
                html += '<div class="meta">';
                html += '<span>' + t.formato + '</span>';
                html += '<span>' + t.participantes + ' participantes</span>';
                html += '</div></div>';
            }
            
            html += '</div>';
            el.innerHTML = html;
        })
        .catch(function() {
            el.innerHTML = '<p style="color:#FF5050;">Error al cargar torneos</p>';
        });
}

function verTorneo(id) {
    fetch(API_BASE + '/api/torneos/' + id)
        .then(function(r) { return r.json(); })
        .then(function(t) {
            var el = document.getElementById('torneo-detalle');
            document.getElementById('torneos-content').style.display = 'none';
            el.style.display = 'block';
            
            var html = '<button class="btn-volver" onclick="volverTorneos()">Volver</button>';
            html += '<h3>' + t.nombre + '</h3>';
            html += '<p style="color:#999;margin-bottom:15px;">Formato: ' + t.formato + ' | Estado: ' + t.estado + '</p>';
            
            if (t.clasificacion && t.clasificacion.length > 0) {
                html += '<div class="tabla-wrapper"><table><thead><tr>';
                html += '<th>Pos</th><th>Jugador</th><th>Puntos</th></tr></thead><tbody>';
                for (var i = 0; i < t.clasificacion.length; i++) {
                    var p = t.clasificacion[i];
                    html += '<tr><td>' + (p.posicion > 0 ? p.posicion : '-') + '</td>';
                    html += '<td>' + p.jugadorNombre + '</td>';
                    html += '<td>' + p.puntosGanados + '</td></tr>';
                }
                html += '</tbody></table></div>';
            } else {
                html += '<p style="color:#999;">Sin participantes aun</p>';
            }
            
            el.innerHTML = html;
        });
}

function volverTorneos() {
    document.getElementById('torneo-detalle').style.display = 'none';
    document.getElementById('torneos-content').style.display = '';
}

// JUGADORES
function cargarJugadores() {
    var el = document.getElementById('jugadores-content');
    document.getElementById('jugador-form').style.display = 'none';
    
    fetch(API_BASE + '/api/jugadores')
        .then(function(r) { return r.json(); })
        .then(function(data) {
            var html = '<div class="tabla-wrapper"><table><thead><tr>';
            html += '<th>ID</th><th>Jugador</th><th>ELO</th><th>Registro</th></tr></thead><tbody>';
            
            for (var i = 0; i < data.length; i++) {
                var j = data[i];
                html += '<tr>';
                html += '<td>' + j.id + '</td>';
                html += '<td><div class="jugador-celda"><img src="' + j.avatar + '" class="avatar-img" alt=""><span>' + j.nombre + '</span></div></td>';
                html += '<td><span class="elo-badge">' + j.elo + '</span></td>';
                html += '<td>' + new Date(j.fechaRegistro).toLocaleDateString() + '</td>';
                html += '</tr>';
            }
            
            html += '</tbody></table></div>';
            el.innerHTML = html;
        })
        .catch(function() {
            el.innerHTML = '<p style="color:#FF5050;">Error al cargar jugadores</p>';
        });
}

function mostrarFormJugador() {
    var el = document.getElementById('jugador-form');
    el.style.display = 'block';
    el.innerHTML = '<div class="form-box">' +
        '<h3>Nuevo Jugador</h3>' +
        '<div class="form-group"><label>Nombre</label><input type="text" id="nuevo-nombre" placeholder="Nombre del jugador"></div>' +
        '<div class="form-group"><label>ELO Inicial</label><input type="number" id="nuevo-elo" value="1000" min="0" max="3000"></div>' +
        '<button class="btn" onclick="crearJugador()">Crear Jugador</button>' +
        '<button class="btn btn-cancelar" onclick="document.getElementById(\'jugador-form\').style.display=\'none\'">Cancelar</button>' +
        '</div>';
}

function crearJugador() {
    var nombre = document.getElementById('nuevo-nombre').value.trim();
    var elo = parseInt(document.getElementById('nuevo-elo').value);
    if (!nombre) { alert('Falta el nombre'); return; }
    
    fetch(API_BASE + '/api/jugadores', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ nombre: nombre, elo: elo })
    })
    .then(function() {
        document.getElementById('jugador-form').style.display = 'none';
        cargarJugadores();
    })
    .catch(function() { alert('Error al crear jugador'); });
}

// STATS
function cargarStats() {
    var el = document.getElementById('stats-content');
    fetch(API_BASE + '/api/stats')
        .then(function(r) { return r.json(); })
        .then(function(s) {
            var html = '<div class="stats-grid">';
            html += '<div class="stat-card"><div class="valor">' + s.totalJugadores + '</div><div class="label">Jugadores</div></div>';
            html += '<div class="stat-card"><div class="valor">' + s.totalTorneos + '</div><div class="label">Torneos</div></div>';
            html += '<div class="stat-card"><div class="valor">' + s.torneosActivos + '</div><div class="label">Activos</div></div>';
            html += '<div class="stat-card"><div class="valor">' + s.mejorElo + '</div><div class="label">Mejor ELO</div></div>';
            html += '<div class="stat-card"><div class="valor">' + Math.round(s.promedioElo) + '</div><div class="label">ELO Promedio</div></div>';
            html += '</div>';
            el.innerHTML = html;
        })
        .catch(function() {
            el.innerHTML = '<p style="color:#FF5050;">Error al cargar estadisticas</p>';
        });
}
