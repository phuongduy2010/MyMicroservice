# Clean Architecture Kafka Microservices (.NET 9)

Services: OrderSvc, InventorySvc, PaymentSvc, ShippingSvc, NotificationSvc.
Architecture per service: **Domain / Application / Infrastructure / Api**.
Messaging: **Kafka** (Redpanda). Persistence: **Postgres** (each service has its own DB).
Reliability: **Outbox/Inbox** pattern per service.

## Quick Start
```bash
docker compose up -d
dotnet run --project src/OrderSvc/Api
dotnet run --project src/InventorySvc/Api
dotnet run --project src/PaymentSvc/Api
dotnet run --project src/ShippingSvc/Api
dotnet run --project src/Notificat'ionSvc/Api
```
Create an order:
```bash
curl -X POST http://localhost:5001/orders   -H "Content-Type: application/json"   -d '{"customerId":"C-1","items":[{"sku":"SKU-1","qty":1},{"sku":"SKU-2","qty":1}]}'
```
