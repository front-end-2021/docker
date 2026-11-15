from fastapi import FastAPI

from app.modules import cart, catalog, orders, payments

for module in (catalog, cart, orders, payments):
    module.init()

app = FastAPI(title="Mini Shop")
for module in (catalog, cart, orders):
    app.include_router(module.router)
