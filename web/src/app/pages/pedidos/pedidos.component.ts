import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

interface Producto {
  id: number;
  sku: string;
  marca: string;
  modelo: string;
}

interface PedidoDetalle {
  id?: number;
  productoId: number;
  cantidad: number;
  producto?: Producto;
}

interface Pedido {
  id: number;
  fecha: string;
  estado: string;
  comentario: string;
  creadoPor: { id: number; nombre: string };
  detalles: PedidoDetalle[];
}

interface PedidoEstadoUpdate {
  estado: string;
  comentario: string;
}

@Component({
  selector: 'app-pedidos',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './pedidos.component.html',
  styleUrls: ['./pedidos.component.css'],
})
export class PedidosComponent implements OnInit {
  apiUrl = 'http://localhost:8080/api/Pedidos';
  apiProductos = 'http://localhost:8080/api/Productos';
  pedidos: Pedido[] = [];
  productos: Producto[] = [];
  nuevoPedido: PedidoDetalle[] = [];

  cargando = false;
  error = '';
  rol = '';
  comentario = '';

  mostrarModal = false;

  constructor(private http: HttpClient) {}

  ngOnInit() {
    const token = localStorage.getItem('token');
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.rol = payload.role || payload.Rol || '';
    }
    this.obtenerPedidos();
  }

  obtenerPedidos() {
    this.cargando = true;
    this.http.get<Pedido[]>(this.apiUrl).subscribe({
      next: (res) => {
        this.pedidos = res;
        this.cargando = false;
      },
      error: () => {
        this.error = 'Error al obtener los pedidos.';
        this.cargando = false;
      },
    });
  }

  aprobarPedido(pedido: Pedido) {
    const comentario = prompt('Comentario de aprobación:') ?? '';
    if (!comentario.trim()) return;

    this.http
      .patch(`${this.apiUrl}/${pedido.id}/aprobar`, JSON.stringify(comentario), {
        headers: { 'Content-Type': 'application/json' },
      })
      .subscribe({
        next: () => this.obtenerPedidos(),
        error: () => alert('Error al aprobar el pedido.'),
      });
  }

  rechazarPedido(pedido: Pedido) {
    const motivo = prompt('Motivo del rechazo:') ?? '';
    if (!motivo.trim()) return;

    this.http
      .patch(`${this.apiUrl}/${pedido.id}/rechazar`, JSON.stringify(motivo), {
        headers: { 'Content-Type': 'application/json' },
      })
      .subscribe({
        next: () => this.obtenerPedidos(),
        error: () => alert('Error al rechazar el pedido.'),
      });
  }

  revisarPedido(pedido: Pedido) {
    const comentario = prompt('Agregar comentario antes de marcar como revisado:') ?? '';
    if (!comentario.trim()) return;

    const payload = { estado: 'Revisado', comentario };

    this.http
      .patch(`${this.apiUrl}/${pedido.id}/estado`, payload, {
        headers: { 'Content-Type': 'application/json' },
      })
      .subscribe({
        next: () => this.obtenerPedidos(),
        error: (err) => {
          console.error(err);
          alert('Error al actualizar el pedido.');
        },
      });
  }

  abrirModal() {
    this.nuevoPedido = [{ productoId: 0, cantidad: 1 }];
    this.mostrarModal = true;

    this.http.get<Producto[]>(this.apiProductos).subscribe({
      next: (res) => (this.productos = res),
      error: () => alert('Error al obtener productos.'),
    });
  }

  agregarFila() {
    this.nuevoPedido.push({ productoId: 0, cantidad: 1 });
  }

  eliminarFila(index: number) {
    this.nuevoPedido.splice(index, 1);
  }

  crearPedido() {
    if (this.nuevoPedido.some((d) => d.productoId === 0 || d.cantidad <= 0)) {
      alert('Debe seleccionar productos válidos y cantidades mayores a cero.');
      return;
    }

    const token = localStorage.getItem('token');
    let creadoPorId = 0;
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]));
      creadoPorId = payload.UserId || payload.userId || 0;
    }

    const pedido = {
      creadoPorId,
      detalles: this.nuevoPedido
    };

    this.http.post(this.apiUrl, pedido).subscribe({
      next: () => {
        this.mostrarModal = false;
        this.obtenerPedidos();
      },
      error: (err) => {
        console.error(err);
        alert('Error al crear el pedido.');
      },
    });
  }

}
