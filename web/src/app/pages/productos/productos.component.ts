import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Producto {
  id: number;
  sku: string;
  marca: string;
  modelo: string;
  estado: string;
}

@Component({
  selector: 'app-productos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './productos.component.html',
})
export class ProductosComponent implements OnInit {
  productos: Producto[] = [];
  productoForm: Partial<Producto> = {};
  editando: Producto | null = null;
  terminoBusqueda = '';
  apiUrl = 'http://localhost:8080/api/Productos';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.obtenerProductos();
  }

  obtenerProductos() {
    this.http.get<Producto[]>(this.apiUrl).subscribe({
      next: data => (this.productos = data),
    });
  }

  buscar() {
    if (!this.terminoBusqueda.trim()) return this.obtenerProductos();
    this.http.get<Producto[]>(`${this.apiUrl}/${this.terminoBusqueda}`).subscribe({
      next: data => (this.productos = data),
    });
  }

  guardar() {
    if (this.editando) {
      this.http.put(`${this.apiUrl}/${this.editando.id}`, this.productoForm).subscribe({
        next: () => {
          this.cancelarEdicion();
          this.obtenerProductos();
        },
      });
    } else {
      this.http.post(this.apiUrl, this.productoForm).subscribe({
        next: () => {
          this.productoForm = {};
          this.obtenerProductos();
        },
      });
    }
  }

  editar(p: Producto) {
    this.editando = p;
    this.productoForm = { ...p };
  }

  eliminar(id: number) {
    if (!confirm('¿Eliminar este producto?')) return;
    this.http.delete(`${this.apiUrl}/${id}`).subscribe({
      next: () => this.obtenerProductos(),
    });
  }

  cancelarEdicion() {
    this.editando = null;
    this.productoForm = {};
  }
}
