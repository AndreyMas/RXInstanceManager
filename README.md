# Описание утилиты RX Instance Manager
![](https://github.com/AndreyMas/RXInstanceManager/releases/download/v0.9.04082022/v0904082022.png)
Утилита позволяет настраивать экземпляры системы Directum RX на серверах разработки прикладных и заказных проектов.

Под экземпляром системы понимается набор, состоящий из:
* базы данных;
* хранилища документов;
* одного или нескольких репозиториев исходного кода.

На данный момент утилита облегчает следующие задачи:
* установка новых экземпляров системы;
* удаление существующих экземпляров;
* запуск и остановка служб установленных экземпляров;
* обновление конфигурации экземпляров;
* запуск проводника системы и Directum Development Studio установленных экземпляров из единого окна.

***Внимание!*** Утилита рассчитана на применение на рабочих местах разработчиков и не предназначена для управления тестовыми и продуктивными серверами.

Текущая версия совместима с версиями DirectumLauncher: 4.2, 4.3, 4.4.

## Установка утилиты
Установка осуществляется из самораспаковывающегося архива: [Последняя версия](https://github.com/AndreyMas/RXInstanceManager/releases/download/v0.9.18082022/RXInstanceManager_18082022.exe).

После распаковки необходимо запустить RCInstanceManager.exe. Рекомендуется вынести ярлык на рабочий стол или закрепить в панели быстрого доступа или на начальном экране в меню "Пуск".

## Установка первого экземпляра
Первый экземпляр должен быть установлен через DirectumLauncher в режиме нескольких инстансов. Для этого необходимо:
1. Запустить DirectumLauncher из папки экземпляра системы;
2. Открыть раздел "Настройка", в блоке variables задать новую переменную instance_name: '<код экземпляра>';
3. Проверить другие переменные в блоке variables (http порт, имя хоста, протокол);
4. **Нажать "Сохранить"!** Иначе система будет установлена в режиме одного экземпляра;
5. Вернуться в раздел "Установка", название точки обмена Rabbit **должно совпадать** с кодом экземпляра;

## Пример организации каталогов для нескольких экземпляров системы

```
C:\RX                            <-------- корневой каталог для всех экземпляров RX (включая хранилища файлов и исходных кодов)
   +--Instances                  <-------- корневой каталог для всех инстансов DirectumLauncher
   +--Sources                    <-------- корневой каталог для исходников прикладных проектов
   +--Storage                    <-------- корневой каталог для домашних каталогов (хранилище документов)

C:\RX\Instances                  <-------- корневой каталог для всех инстансов DirectumLauncher
   +--dtreqs43                   <-------- каталог инстанса проекта "Решение Требования" под RX 4.3.0.0083. Имя инстанса: dtreqs43
   +--sandbox44                  <-------- каталог инстанса проекта "Песочница 4.4." под RX 4.4.0.0076. Имя инстанса: sandbox44
   
C:\RX\Sources                    <-------- корневой каталог для исходников прикладных проектов
   +--dtreqs43                   <-------- каталог для исходников проекта "Решение Требования"
   |    +--Base                  <-------- каталог для исходников базового слоя проекта "Решение Требования"
   |    +--Work                  <-------- каталог для исходников рабочего слоя проекта "Решение Требования"
   +--sandbox44                  <-------- каталог для исходников проекта "Песочница 4.4."
   |    +--Base                  <-------- каталог для исходников базового слоя проекта "Песочница 4.4."
   |    +--Work                  <-------- каталог для исходников рабочего слоя проекта "Песочница 4.4."
   
C:\RX\Storage                    <-------- корневой каталог для домашних каталогов (хранилище документов)
   +--dtreqs43                   <-------- каталог для домашних каталогов проекта "Решение Требования"
   +--sandbox44                  <-------- каталог для домашних каталогов проекта "Песочница 4.4."
   
```

## Работа с утилитой
* Краткая инструкция работы с утилитой доступна по кнопке "*Инструкция*".
* Для добавления нового инстанса необходимо нажать на кнопку "*Добавить*".
* Для установки добавленного инстанса необходимо нажать на кнопку "*Установить*".
* Для запуска проводника системы необходимо нажать на кнопку "*Запустить RX*" (система должна быть установлена).
* Для запуска Directum Development Studio необходимо нажать на кнопку "*Запустить DDS*" (система должна быть установлена).
* Для запуска остановленной службы экземпляра необходимо нажать на кнопку контекстного меню "*Запустить службу*".
* Для остановки запущенной службы экземпляра необходимо нажать на кнопку контекстного меню "*Остановить службу*".
* Для редактирования конфигурационного файла экземпляра необходимо нажать на кнопку контекстного меню "*Открыть config.yml*".
* Для перезапуска службы (принятия изменений конфигурационного файла) экземпляра необходимо нажать на кнопку контекстного меню "*Пересобрать из config.yml*".
