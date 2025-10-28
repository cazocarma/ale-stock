import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

interface PedidoDetalle {
  id: number;
  productoId: number;
  cantidad: number;
  producto: {
    id: number;
    sku: string;
    marca: string;
    modelo: string;
  };
}

interface Pedido {
  id: number;
  fecha: string;
  estado: string;
  comentario: string;
  creadoPor: { id: number; nombre: string };
  detalles: PedidoDetalle[];
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
  pedidos: Pedido[] = [];
  cargando = false;
  error = '';
  rol = '';
  comentario = '';

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

  cambiarEstado(pedido: Pedido) {
    const nuevo = prompt('Nuevo estado (ej. En tránsito, Entregado):') ?? '';
    if (!nuevo.trim()) return;

    this.http
      .patch(`${this.apiUrl}/${pedido.id}/estado`, JSON.stringify(nuevo), {
        headers: { 'Content-Type': 'application/json' },
      })
      .subscribe({
        next: () => this.obtenerPedidos(),
        error: () => alert('Error al cambiar el estado.'),
      });
  }
}
