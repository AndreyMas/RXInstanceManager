1. Для развертывания инстанса необходимо:
  1.1. Разархивировать DirectumLauncher в папку где будет установлена система;
  1.2. Скопировать в папку архивы Platform, Redist (для 4.5 и выше), Development Studio, Deployment Tool, DirectumRX;
  1.3. В менеджере экземпляров системы нажать кнопку "Добавить";
  1.4. Выбрать папку в которую разархивирован DirectumLauncher (п. 1.1);
  1.5. Указать новый код системы;
  1.6. Заполнить блок variables в config.yml;
  1.7. Нажать кнопку "Установить", запустится DirectumLauncher, в DL выбрать раздел "Установка";
  1.8. Название точки обмена Rabbit должно быть уникальной (напр., совпадать с кодом экземпляра), выполнить установку;
  1.9. Проверить config.yml, при необходимости скорректировать (напр., репозитории), нажать "Пересобрать из config.yml".
2. После удаления инстанса необходимо:
  2.1. Вручную удалить: пул приложений, базу данных.