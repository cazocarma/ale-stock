import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-movimientos',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIf, NgFor, DatePipe],
  templateUrl: './movimientos.component.html',
})
export class MovimientosComponent implements OnInit {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  movimientos: any[] = [];
  filtroProductoId: number | null = null;
  fechaInicio: string = '';
  fechaFin: string = '';
  resumen: any | null = null;
  cargando = false;
  apiUrl = 'http://localhost:8080/api/Movimientos';

  ngOnInit() {
    this.cargarMovimientos();
  }

  cargarMovimientos() {
    this.cargando = true;
    this.http.get<any[]>(this.apiUrl).subscribe({
      next: (res) => {
        this.movimientos = res;
        this.cargando = false;
      },
      error: () => (this.cargando = false),
    });
  }

  filtrarPorProducto() {
    if (!this.filtroProductoId) return;
    this.cargando = true;
    this.http.get<any[]>(`${this.apiUrl}/producto/${this.filtroProductoId}`).subscribe({
      next: (res) => {
        this.movimientos = res;
        this.cargando = false;
      },
      error: () => (this.cargando = false),
    });
  }

  filtrarPorRango() {
    if (!this.fechaInicio || !this.fechaFin) return;
    this.cargando = true;
    this.http
      .get<any[]>(`${this.apiUrl}/rango?inicio=${this.fechaInicio}&fin=${this.fechaFin}`)
      .subscribe({
        next: (res) => {
          this.movimientos = res;
          this.cargando = false;
        },
        error: () => (this.cargando = false),
      });
  }

  obtenerResumen() {
    if (!this.filtroProductoId) return;
    this.cargando = true;
    this.http.get<any>(`${this.apiUrl}/resumen/${this.filtroProductoId}`).subscribe({
      next: (res) => {
        this.resumen = res;
        this.cargando = false;
      },
      error: () => (this.cargando = false),
    });
  }
}
