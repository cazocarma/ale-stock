import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';

interface Inventario {
  id: number;
  productoId: number;
  cantidad: number;
  ubicacion: string;
  producto: {
    id: number;
    sku: string;
    marca: string;
    modelo: string;
    estado: string;
  };
}

interface MovimientoRequest {
  productoId: number;
  cantidad: number;
  comentario: string;
  usuarioId: number;
}

@Component({
  selector: 'app-inventario',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './inventario.component.html',
  styleUrls: ['./inventario.component.css']
})
export class InventarioComponent implements OnInit {
  apiUrl = 'http://localhost:8080/api/Inventario';
  inventario: Inventario[] = [];
  filtro = '';
  stockMinimo = 5;
  movimiento: MovimientoRequest = { productoId: 0, cantidad: 0, comentario: '', usuarioId: 1 };
  mostrarModal = false;
  tipoMovimiento: 'entrada' | 'salida' = 'entrada';
  cargando = false;
  error = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.obtenerInventario();
  }

  obtenerInventario() {
    this.cargando = true;
    this.http.get<Inventario[]>(this.apiUrl).subscribe({
      next: (res) => {
        this.inventario = res;
        this.cargando = false;
      },
      error: () => {
        this.error = 'Error al obtener el inventario.';
        this.cargando = false;
      }
    });
  }

  filtrarInventario() {
    if (!this.filtro) return this.inventario;
    return this.inventario.filter((i) =>
      i.producto.marca.toLowerCase().includes(this.filtro.toLowerCase()) ||
      i.producto.modelo.toLowerCase().includes(this.filtro.toLowerCase()) ||
      i.producto.sku.toLowerCase().includes(this.filtro.toLowerCase())
    );
  }

  mostrarAjuste(producto: Inventario, tipo: 'entrada' | 'salida') {
    this.movimiento = {
      productoId: producto.producto.id,
      cantidad: 0,
      comentario: '',
      usuarioId: 1 // Temporal: el usuario logueado debería venir del token
    };
    this.tipoMovimiento = tipo;
    this.mostrarModal = true;
  }

  guardarAjuste() {
    if (!this.movimiento.cantidad || this.movimiento.cantidad <= 0) {
      alert('Debe ingresar una cantidad válida.');
      return;
    }
    if (!this.movimiento.comentario.trim()) {
      alert('Debe ingresar un comentario.');
      return;
    }

    const cantidadFinal =
      this.tipoMovimiento === 'salida'
        ? -Math.abs(this.movimiento.cantidad)
        : Math.abs(this.movimiento.cantidad);

    const movimientoFinal = { ...this.movimiento, cantidad: cantidadFinal };

    this.http.post(`${this.apiUrl}/ajustar`, movimientoFinal).subscribe({
      next: () => {
        this.mostrarModal = false;
        this.obtenerInventario();
      },
      error: (err) => {
        console.error(err);
        alert('Error al ajustar el stock.');
      }
    });
  }

  obtenerBajoMinimo() {
    this.http.get<Inventario[]>(`${this.apiUrl}/bajo-minimo/${this.stockMinimo}`).subscribe({
      next: (res) => (this.inventario = res),
      error: () => (this.error = 'Error al obtener productos bajo mínimo.')
    });
  }
}
