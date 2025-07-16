// ...existing code...
public class PedidoRepository : IPedidoRepository
{
    // ...existing code...

    public void Atualizar(Pedido pedido)
    {
        // Assuming _context is your DbContext and Pedidos is your DbSet<Pedido>
        _context.Pedidos.Update(pedido);
        _context.SaveChanges();
    }

    // ...existing code...
}
// ...existing code...
