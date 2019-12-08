# .NET Client for PayKickstart API

This is very early version. Not all API calls are implemented. Contributors are welcome to contribute remaining API calls.

## Use cases

### Prorated upgrade




## API
Сам API надо описать, но это больше справочная информация. Лучше описать типовые сценарии, которые можно сделать реализовать с помощью библиотеки. Потому как местами пояснение как самим API пользоваться. Поэтому
лучше подавать в виде use-cases, а не просто доступных функций. Пример - Newton.Json.


### Direct API calls
GetPurchaseJsonAsync
NewPurchaseJsonAsync

CancelSubscriptionJsonAsync


BillingCustomerLoginJsonAsync


### Additional functionality, not directly provided by API
ProratedSubscriptionUpgradeAsync - по сути этот метод просто делает новый Purchase, устанавливая trial-поля. Но он не отменяет предыдущую подписку (так как сначала надо получить оплату). Он сам не умеет считать разницу между какой-то другой подпиской и новой. В общем этот метод надо или переназвать, или как-то переобдумать что он делает, что бы он более полноценно выполнял переключение с одной подписку на другую, более дорогую.
CancelSubscriptionAtEndJsonAsync - gets subscription's next_date and sets cancel date to "next_date at 00:00:01".


### Subscriptions
ReactivateSubscription


## Design notes
Все методы возвращают JSON ответа. Поэтому они имеют суфикс Json. Это сделано для большей универсальности библиотеки: если в PK изменит/добавит формат ответа, а библиотека еще не обновится, то можно будет работать напрямую с raw JSON, возвращаемым web API.

Есть еще зачатки объектной модели.
Purchase
    GetSubscriptionRemainingAmount()
Subscription
Transaction



